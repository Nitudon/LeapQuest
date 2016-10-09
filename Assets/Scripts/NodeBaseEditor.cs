using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace NodeBaseEditor
{
    /// <summary>
    /// 接続点の位置を表す定数
    /// </summary>
    public enum JunctionPosition
    {
        LeftTop = 0,
        RightTop,
        LeftBottom,
        RightBottom
    }

    /// <summary>
    /// ノードの色を表す定数
    /// </summary>
    public enum NodeColor
    {
        // 組み込みスキンの末尾の数字を定数の値を元に算出できるようにしておく
        Blue = 1,
        Green = 3,
        Yellow = 4,
        Orange = 5,
        Red = 6
    }

    /// <summary>
    /// ノードのIDの払い出し
    /// </summary>
    public static class NodeId
    {
        static long id = 1;

        public static long Create()
        {
            // ただ整数をインクリメントして返すだけ
            return id++;
        }
    }

    public class NodeWindow : EditorWindow
    {
        Dictionary<int, Node> nodes = new Dictionary<int, Node>();

        string inputNodeText = string.Empty;
        Texture2D inputNodeTexture = null;
        NodeColor selectedColor = NodeColor.Blue;

        [MenuItem("NodeEditorWindow/Open...")]
        public static void Open()
        {
            var window = GetWindow<NodeWindow>();
            window.minSize = new Vector2(600f, 300f);
            window.Init();
        }

        void Init()
        {
            wantsMouseMove = true;
            ConnectorManager.Init();
        }

        void OnGUI()
        {
            // ノードの描画
            BeginWindows();
            foreach (var node in nodes.Values)
            {
                node.Update();
            }
            EndWindows();

            // 決定中の接続がある場合は右クリックでキャンセル
            var ev = Event.current;
            if (ConnectorManager.HasCurrent && ev.type == EventType.mouseDown && ev.button == 1)
            {
                ConnectorManager.CancelConnecting();
            }

            ConnectorManager.Update(Event.current.mousePosition);
            if (ConnectorManager.HasCurrent)
            {
                Repaint(); // 関連付けようとしている接続がある場合は描画する
            }

            // ノードを作成するための左カラムを描画していく
            EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true), GUILayout.Width(300));

            selectedColor = (NodeColor)EditorGUILayout.EnumPopup("Node color: ", selectedColor);

            // テキストを表示するノードの作成
            EditorGUILayout.BeginHorizontal();
            inputNodeText = EditorGUILayout.TextField("Text node: ", inputNodeText, GUILayout.ExpandWidth(true));
            GUI.enabled = !string.IsNullOrEmpty(inputNodeText);
            if (GUILayout.Button("Create", GUILayout.Width(60)))
            {
                var node = new TextNode(inputNodeText, selectedColor);
                nodes.Add(node.Id, node);
                inputNodeText = string.Empty;
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            // テクスチャを表示するノードの作成
            EditorGUILayout.BeginHorizontal();
            inputNodeTexture = EditorGUILayout.ObjectField("Texture node: ", inputNodeTexture, typeof(Texture2D), false, GUILayout.ExpandWidth(true)) as Texture2D;
            GUI.enabled = inputNodeTexture != null;
            if (GUILayout.Button("Create", GUILayout.Width(60)))
            {
                var node = new TextureNode(inputNodeTexture, selectedColor);
                nodes.Add(node.Id, node);
                inputNodeTexture = null;
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
    }

    /// <summary>
    /// ノードの接続を管理するクラス
    /// </summary>
    public static class ConnectorManager
    {
        static List<Connector> connectors;
        static Dictionary<int, Dictionary<JunctionPosition, Connector>> connected;
        static Connector current;

        /// <summary>
        /// あるノードを始点にして接続を作成
        /// </summary>
        /// <param name="startNode">始点となるノード</param>
        /// <param name="startPosition">ノードの接点の位置</param>
        public static void StartConnecting(Node startNode, JunctionPosition startPosition)
        {
            if (current != null)
            {
                throw new UnityException("Already started connecting.");
            }

            if (connected.ContainsKey(startNode.Id) && connected[startNode.Id].ContainsKey(startPosition))
            {
                throw new UnityException("Already connected node.");
            }

            current = new Connector(startNode, startPosition);
        }

        public static void CancelConnecting()
        {
            current = null;
        }

        public static bool HasCurrent
        {
            get
            {
                return current != null;
            }
        }

        public static bool IsCurrent(Node node, JunctionPosition position)
        {
            return HasCurrent && current.StartNode.Id == node.Id && current.StartPosition == position;
        }

        /// <summary>
        /// 終点となるノードを決定
        /// </summary>
        /// <param name="endNode">終点となるノード</param>
        /// <param name="endPosition">ノードの接点の位置</param>
        public static void Connect(Node endNode, JunctionPosition endPosition)
        {
            if (current == null)
            {
                throw new UnityException("No current connector.");
            }

            current.Connect(endNode, endPosition);
            connectors.Add(current);

            // 接続情報を登録
            if (!connected.ContainsKey(current.StartNode.Id))
            {
                connected[current.StartNode.Id] = new Dictionary<JunctionPosition, Connector>();
            }
            connected[current.StartNode.Id].Add(current.StartPosition, current);

            if (!connected.ContainsKey(current.EndNode.Id))
            {
                connected[current.EndNode.Id] = new Dictionary<JunctionPosition, Connector>();
            }
            connected[current.EndNode.Id].Add(current.EndPosition, current);

            current = null;
        }

        /// <summary>
        /// あるノードの接続点に接続されている接続を返します
        /// </summary>
        /// <param name="node">ノード</param>
        /// <param name="position">接続点の位置</param>
        /// <returns>接続. 接続されていない場合はnull</returns>
        public static Connector GetConnector(Node node, JunctionPosition position)
        {
            if (connected.ContainsKey(node.Id) && connected[node.Id].ContainsKey(position))
            {
                return connected[node.Id][position];
            }
            else
            {
                return null;
            }
        }

        public static bool IsConnected(Node node, JunctionPosition position)
        {
            return GetConnector(node, position) != null;
        }

        /// <summary>
        /// ある接続点に接続されている接続を解除します
        /// </summary>
        /// <param name="node">始点若しくは終点として接続されているノード</param>
        /// <param name="position">接続点の位置</param>
        public static void Disconnect(Node node, JunctionPosition position)
        {
            var con = GetConnector(node, position);
            if (con == null)
            {
                return;
            }

            for (int i = 0; i < connectors.Count; i++)
            {
                var other = connectors[i];
                if (con.StartNode.Id == other.StartNode.Id && con.StartPosition == other.StartPosition &&
                    con.EndNode.Id == other.EndNode.Id && con.EndPosition == other.EndPosition)
                {
                    connectors.RemoveAt(i);
                    break;
                }
            }

            connected[con.StartNode.Id].Remove(con.StartPosition);
            connected[con.EndNode.Id].Remove(con.EndPosition);
        }

        public static void Init()
        {
            connectors = new List<Connector>();
            connected = new Dictionary<int, Dictionary<JunctionPosition, Connector>>();
            current = null;
        }

        /// <summary>
        /// 管理している接続の描画
        /// </summary>
        /// <param name="mousePosition">マウスの位置情報</param>
        public static void Update(Vector2 mousePosition)
        {
            connectors.ForEach(con => con.Draw());

            if (current != null)
            {
                current.DrawTo(mousePosition);
            }
        }
    }

    /// <summary>
    /// ノード間の接続を表すクラス
    /// </summary>
    public class Connector
    {
        readonly Color color = Color.gray;

        public Node StartNode { get; private set; }
        public JunctionPosition StartPosition { get; private set; }

        public Node EndNode { get; private set; }
        public JunctionPosition EndPosition { get; private set; }

        public Connector(Node node, JunctionPosition position)
        {
            StartNode = node;
            StartPosition = position;
        }

        public void Connect(Node node, JunctionPosition position)
        {
            EndNode = node;
            EndPosition = position;
        }

        /// <summary>
        /// 接続を曲線として描画
        /// </summary>
        public void Draw()
        {
            if (EndNode == null)
            {
                throw new UnityException("No end node.");
            }

            var start = StartNode.CalculateConnectorPoint(StartPosition);
            var startV3 = new Vector3(start.x, start.y, 0f);
            var startTan = new Vector3((int)StartPosition % 2 == 0 ? start.x - 100f : start.x + 100f, start.y, 0f);

            var end = EndNode.CalculateConnectorPoint(EndPosition);
            var endV3 = new Vector3(end.x, end.y, 0f);
            var endTan = new Vector3((int)EndPosition % 2 == 0 ? end.x - 100f : end.x + 100f, end.y, 0f);

            Handles.DrawBezier(startV3, endV3, startTan, endTan, color, null, 4f);
        }

        /// <summary>
        /// 始点となるノードと, 指定の座標を結ぶ直線の描画
        /// 終点の決定中に始点とマウス間の直線を描画する際に使う
        /// </summary>
        /// <param name="end">描画する直線の終点</param>
        public void DrawTo(Vector2 to)
        {
            var start = StartNode.CalculateConnectorPoint(StartPosition);
            Handles.DrawLine(new Vector3(start.x, start.y, 0f), new Vector3(to.x, to.y, 0f));
        }
    }

    /// <summary>
    /// ノードの基底クラス
    /// ノード自身の描画や接続点の管理を行う
    /// </summary>
    public abstract class Node
    {
        protected readonly Vector2 JunctionSize = new Vector2(15f, 15f);

        int id;
        Rect rect;
        NodeColor color;

        public int Id { get { return id; } }

        public Rect Rect { get { return rect; } }

        public Node(Rect rect, NodeColor color)
        {
            id = (int)NodeId.Create();
            this.rect = rect;
            this.color = color;
        }

        public void Update()
        {
            // 組み込みのスキンを使ってノードを描画
            rect = GUI.Window(id, rect, WindowCallback, string.Empty, "flow node " + ((int)color).ToString());
        }

        /// <summary>
        /// ウィンドウ内のGUI(接続点等)の描画
        /// </summary>
        void WindowCallback(int id)
        {
            foreach (JunctionPosition position in Enum.GetValues(typeof(JunctionPosition)))
            {
                var style = (int)position % 2 == 0 ? "LargeButtonRight" : "LargeButtonLeft";
                if (ConnectorManager.HasCurrent)
                {
                    // 決定中の接続がある場合は始点となっている場合, 既に接続済みである場合に非アクティブ
                    GUI.enabled = !ConnectorManager.IsConnected(this, position) && !ConnectorManager.IsCurrent(this, position);
                    if (GUI.Button(CalculateJunctionRect(position), string.Empty, style))
                    {
                        ConnectorManager.Connect(this, position); // クリックされたら接続
                    }
                    GUI.enabled = true;
                }
                else
                {
                    if (GUI.Button(CalculateJunctionRect(position), string.Empty, style))
                    {
                        if (ConnectorManager.IsConnected(this, position))
                        {
                            ConnectorManager.Disconnect(this, position);
                        }
                        else
                        {
                            ConnectorManager.StartConnecting(this, position);
                        }
                    }
                }
            }

            OnGUI();
            GUI.DragWindow();
        }

        // ノードの種別毎のUIは子クラスで実装
        abstract protected void OnGUI();

        /// <summary>
        /// 接続点の描画位置を計算して返す
        /// </summary>
        /// <param name="position">接続点の位置</param>
        /// <returns>接続点の描画位置を表す矩形</returns>
        Rect CalculateJunctionRect(JunctionPosition position)
        {
            var isLeft = (int)position % 2 == 0;
            var x = isLeft ? 0f : rect.width - JunctionSize.x;
            var y = rect.height / 3f * (Mathf.Floor((int)position / 2f) + 1) - JunctionSize.y / 2f;

            return new Rect(x, y, JunctionSize.x, JunctionSize.y);
        }

        /// <summary>
        /// 接続点を結ぶ接続を描画する際の始点若しくは終点の座標位置を計算して返す
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector2 CalculateConnectorPoint(JunctionPosition position)
        {
            var junction = CalculateJunctionRect(position);
            var x = (int)position % 2 == 0 ? junction.x : junction.x + junction.width;
            var y = junction.y + JunctionSize.y / 2f;

            return new Vector2(x + rect.x, y + rect.y); // ノード(ウィンドウ)の位置を加算して返す
        }
    }

    /// <summary>
    /// テキストを表示するノード
    /// </summary>
    public class TextNode : Node
    {
        string text;

        public TextNode(string text, NodeColor color) : base(new Rect(310, 10, 200, 60), color)
        {
            this.text = text;
        }

        protected override void OnGUI()
        {
            var style = EditorStyles.label;
            var defaultAlignment = style.alignment;
            style.alignment = TextAnchor.MiddleCenter;

            var rect = new Rect(JunctionSize.x, 0, Rect.width - JunctionSize.x * 2, Rect.height);
            GUI.Label(rect, text, style);

            style.alignment = defaultAlignment;
        }
    }

    /// <summary>
    /// テクスチャを表示するノード
    /// </summary>
    public class TextureNode : Node
    {
        Texture2D texture;

        public TextureNode(Texture2D texture, NodeColor color) : base(new Rect(310, 10, 150, 150), color)
        {
            this.texture = texture;
        }

        protected override void OnGUI()
        {
            var padding = JunctionSize.x + 10f;
            var rect = new Rect(padding, padding, Rect.width - padding * 2, Rect.height - padding * 2);

            EditorGUI.DrawPreviewTexture(rect, texture, null, ScaleMode.ScaleAndCrop);
        }
    }
}
