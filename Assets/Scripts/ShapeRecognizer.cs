/*
The MIT License (MIT)
Copyright (c) 2014 hecomi
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

#define DEBUG

using UnityEngine;
using System.Collections.Generic;

public class ShapeRecognizer : MonoBehaviour
{

    [SerializeField]
    private GameObject Sphere;

    public enum ShapeType { Circle, Triangle, Rectangle, Star }
    public struct Shape
    {
        public ShapeType type;
        public Vector3 position;
        public Vector3 normal;
        public Vector3 up;
        public Quaternion rotation;
        public Vector3 scale;
        public List<Vector3> trail;
    }

    #region [Public Parameters]
    public int positionCacheNum = 180;    // 格納する最小点数（60 fps なら 180 で 3 sec 分保存）

    public float minCircleError = 0.07f;  // 円の誤差の総和の許容値（大きいほど適当な図形でも反応する）
    public float minCircleRadius = 0.12f;  // 円と認識する最小半径

    public float minPolygonalSideLength = 0.2f;   // 多角形の辺の最小値
    public int stopPointJudgePointNum = 4;      // 停止点と判断するための速度を算出する点数
    public float stopPointMaxVelocity = 0.006f; // これ以下の速度ならば停止点と認識する

    public int sharpAngleJudgePointNum = 6;      // 鋭角認識のための辺と判断する点数（実際はこの４倍の点を見る）
    public float sharpAngleJudgeSideLength = 0.05f;

    public float closeJudgeDistance = 0.2f;   // 図形が終端したと判断する誤差
    public float adjacentDistanceThreshold = 0.05f;  // 前回の距離との差が小さい時は円の判定を除外（軽量化のため）
    #endregion


    #region [EventHandlers]
    public delegate void ShapeRecognizedEventHandler(Shape shape);
    public delegate void VertexDetectedEventHandler(Vector3 position);

    public event ShapeRecognizedEventHandler onShapeDetected = shape => { };
    public event VertexDetectedEventHandler onVertexDetected = pos => { };
    #endregion


    #region [Private Parameters]
    private List<Vector3> positions_ = new List<Vector3>();
    private List<Vector3> vertexPoints_ = new List<Vector3>();
    private Vector3 startPosition = new Vector3();
    private Vector3 getCircleCenter = new Vector3();
    private Mesh mesh_;
    private float getCircleRadius = 0f;
    private int skipCountForSharpAngleDetection_ = 0;
    private int resetCount = 0;
    #endregion


    void Start()
    {
        mesh_ = new Mesh();
    }

    void Update()
    {
        AddPositionCache(transform.position);
        if (DetectCircleGesture(positions_.Count))
        {
            Debug.Log("circle");
            Sphere.transform.position = getCircleCenter;
            var _sphere = Instantiate(Sphere) as GameObject;

        }
    }


    void AddPositionCache(Vector3 position)
    {

        if(positions_.Count == 0)
        {
            if (positions_.Count == 0)
            {
                startPosition = position;
            }
            positions_.Insert(0, position);
        }


      else  if (Vector3.Distance(position, positions_[0]) > 0.007f)
        {
            if (positions_.Count == 0)
            {
                startPosition = position;
            }
            positions_.Insert(0, position);
            if (positions_.Count > positionCacheNum)
            {
                positions_.RemoveAt(positions_.Count-1);
            }
        }

        else
        {
            resetCount++;
            if (resetCount > 20)
            {
                resetCount = 0;
                Reset();
            }
        }
    }


    void Reset()
    {
        positions_.Clear();
        vertexPoints_.Clear();
    }


    // 円の検出
    // 円はすべての点の平均点（= 中心）から各点の距離（= 半径）が一定という特性を使って検出
    private bool DetectCircleGesture(int PointSum)
    {
        if (PointSum > 20 && Vector3.Distance(positions_[0],positions_[PointSum-1]) < 0.05f)
        {

            var positionSum = Vector3.zero;

            for(int i=0; i < PointSum;++i)
            positionSum += positions_[i];

            // 過去 i 点の位置の平均（円であれば円の中心点）
            var meanPosition = positionSum / PointSum;

            // 過去 i 点それぞれの点と上記平均点との距離の平均（円であれば半径）
            var meanDistanceSum = 0f;
            for (int i = 0; i < PointSum; ++i)
            {
                meanDistanceSum += Vector3.Distance(positions_[i], meanPosition);
            }
            var meanDistance = meanDistanceSum / PointSum;

            // 各平均点との距離の誤差を正規化して足し合わせた総和
            var error = 0f;
            for (int i = 0; i < PointSum; ++i)
            {
                error += Mathf.Abs(Vector3.Distance(positions_[i], meanPosition) - meanDistance);
            }

            // 誤差の総和が許容値以下で、半径が最低サイズよりも大きかったら円として認識
            Debug.Log("error:" + error + "  meanDistance:" + meanDistance);

            if (error < minCircleError && meanDistance > minCircleRadius)
            {

                getCircleCenter = meanPosition;
                getCircleRadius = meanDistance;

                // 円の法線方向は隣接する 3 点を使った外積の平均として算出
                var circleNormal = Vector3.zero;
                for (var i = 2; i < PointSum; ++i)
                {
                    var v0 = positions_[i - 0] - positions_[i - 1];
                    var v1 = positions_[i - 1] - positions_[i - 2];
                    circleNormal += Vector3.Cross(v0, v1).normalized;
                }
                circleNormal = circleNormal.normalized;

                // 法線の向きはカメラの前方にする
                if (Vector3.Dot(circleNormal, Camera.main.transform.forward) < 0f)
                {
                    circleNormal *= -1;
                }

                // 円と判断した点群を格納
                var trail = new List<Vector3>();
                for (int i = 0; i < PointSum; ++i)
                {
                    trail.Add(positions_[i]);
                }

                // 上向きの位置（円の開始点と中心を結ぶベクトル、円ではあまり関係ない）
                var vector1 = (positions_[0] - meanPosition).normalized;
                var vector2 = (positions_[PointSum / 4] - meanPosition).normalized;
                var up = FindBestFitUpAxis(0, vector1, vector2, circleNormal);

                //mesh_.vertices = trail.ToArray();

                //// UV座標の指定
                //mesh_.uv = new Vector2[] {
                //    new Vector2(0, 0),
                //    new Vector2(1, 0),
                //    new Vector2(1, 1),
                //    new Vector2(0, 1),
                //};
                //// 頂点インデックスの指定
                //mesh_.triangles = new int[] {
                //    0, 1, 2,
                //    0, 2, 3,
                //};

                //mesh_.RecalculateNormals();
                //mesh_.RecalculateBounds();

                //Sphere.GetComponent<MeshFilter>().mesh = mesh_;

                // イベントハンドラを呼ぶ
                onShapeDetected(new Shape()
                {
                    type = ShapeType.Circle,
                    position = meanPosition,
                    rotation = Quaternion.LookRotation(circleNormal, up),
                    normal = circleNormal,
                    up = up,
                    scale = Vector3.one * meanDistance * 2,
                    trail = trail
                });

                // 過去の点履歴を消去
                Reset();

                return true;
            }
        }
        return false;
    }
    Vector3 FindBestFitUpAxis(int polygon, Vector3 firstVertexVector, Vector3 secondVertexVector, Vector3 normal)
    {
        // 基準となる上方向ベクトル
        var up = Vector3.up;

        // 基準となる上方向ベクトルの認識した図形への正射影ベクトル
        var upAxisOnShape = (Vector3.Dot(firstVertexVector, up) * firstVertexVector +
                             Vector3.Dot(secondVertexVector, up) * secondVertexVector).normalized;
        var axis = firstVertexVector;
        var bestFitUpAxis = upAxisOnShape;
        var maxInnerProduct = 0f;
        for (var i = 0; i < polygon; ++i)
        {
            var innerProduct = Vector3.Dot(axis, upAxisOnShape);
            if (innerProduct > maxInnerProduct)
            {
                bestFitUpAxis = axis;
                maxInnerProduct = innerProduct;
            }
            axis = Quaternion.AngleAxis(360 / polygon, normal) * axis;
        }
        return bestFitUpAxis;
    }
}