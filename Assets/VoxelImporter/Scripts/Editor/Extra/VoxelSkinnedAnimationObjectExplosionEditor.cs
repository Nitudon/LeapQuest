﻿using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace VoxelImporter
{
    [CustomEditor(typeof(VoxelSkinnedAnimationObjectExplosion))]
    public class VoxelSkinnedAnimationObjectExplosionEditor : VoxelBaseExplosionEditor
    {
        public VoxelSkinnedAnimationObjectExplosion explosionObject { get; protected set; }
        public VoxelSkinnedAnimationObjectExplosionCore explosionObjectCore { get; protected set; }

        protected override void OnEnable()
        {
            base.OnEnable();

            explosionBase = explosionObject = target as VoxelSkinnedAnimationObjectExplosion;
            if (explosionObject == null) return;
            explosionCore = explosionObjectCore = new VoxelSkinnedAnimationObjectExplosionCore(explosionObject);
        }

        protected override void Inspector_MeshMaterial()
        {
            var prefabType = PrefabUtility.GetPrefabType(explosionObject.gameObject);
            var prefabEnable = prefabType == PrefabType.Prefab || prefabType == PrefabType.PrefabInstance || prefabType == PrefabType.DisconnectedPrefabInstance;

            #region Mesh
            {
                if (explosionObject.meshes != null)
                {
                    if (prefabEnable)
                    {
                        bool contains = true;
                        for (int i = 0; i < explosionObject.meshes.Count; i++)
                        {
                            if (explosionObject.meshes[i] == null || explosionObject.meshes[i].mesh == null || !AssetDatabase.Contains(explosionObject.meshes[i].mesh))
                                contains = false;
                        }
                        EditorGUILayout.LabelField("Mesh", contains ? EditorStyles.boldLabel : guiStyleRedBold);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Mesh", EditorStyles.boldLabel);
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("Mesh", guiStyleMagentaBold);
                }
                {
                    EditorGUI.indentLevel++;
                    {
                        if (explosionObject.meshes != null)
                        {
                            for (int i = 0; i < explosionObject.meshes.Count; i++)
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUI.BeginDisabledGroup(true);
                                    EditorGUILayout.ObjectField(explosionObject.meshes[i].mesh, typeof(Mesh), false);
                                    EditorGUI.EndDisabledGroup();
                                }
                                if (explosionObject.meshes[i] != null && explosionObject.meshes[i].mesh != null)
                                {
                                    if (!AssetDatabase.Contains(explosionObject.meshes[i].mesh))
                                    {
                                        if (GUILayout.Button("Save", GUILayout.Width(48)))
                                        {
                                            #region Create Mesh
                                            string path = EditorUtility.SaveFilePanel("Save mesh", explosionCore.voxelBaseCore.GetDefaultPath(), string.Format("{0}_explosion_mesh{1}.asset", explosionObject.gameObject.name, i), "asset");
                                            if (!string.IsNullOrEmpty(path))
                                            {
                                                if (path.IndexOf(Application.dataPath) < 0)
                                                {
                                                    SaveInsideAssetsFolderDisplayDialog();
                                                }
                                                else
                                                {
                                                    Undo.RecordObject(explosionObject, "Save Mesh");
                                                    path = path.Replace(Application.dataPath, "Assets");
                                                    AssetDatabase.CreateAsset(Mesh.Instantiate(explosionObject.meshes[i].mesh), path);
                                                    explosionObject.meshes[i].mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                    {
                                        if (GUILayout.Button("Reset", GUILayout.Width(48)))
                                        {
                                            #region Reset Mesh
                                            Undo.RecordObject(explosionObject, "Reset Mesh");
                                            explosionObject.meshes[i].mesh = null;
                                            explosionCore.Generate();
                                            #endregion
                                        }
                                    }
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                    }
                    EditorGUI.indentLevel--;
                }
            }
            #endregion

            #region Material
            {
                if (explosionObject.materials != null)
                {
                    if (prefabEnable)
                    {
                        bool contains = true;
                        for (int i = 0; i < explosionObject.materials.Count; i++)
                        {
                            if (explosionObject.materials[i] == null || !AssetDatabase.Contains(explosionObject.materials[i]))
                                contains = false;
                        }
                        EditorGUILayout.LabelField("Material", contains ? EditorStyles.boldLabel : guiStyleRedBold);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Material", EditorStyles.boldLabel);
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("Material", guiStyleMagentaBold);
                }
                {
                    EditorGUI.indentLevel++;
                    {
                        if (explosionObject.materials != null)
                        {
                            for (int i = 0; i < explosionObject.materials.Count; i++)
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUI.BeginDisabledGroup(true);
                                    EditorGUILayout.ObjectField(explosionObject.materials[i], typeof(Material), false);
                                    EditorGUI.EndDisabledGroup();
                                }
                                if (explosionObject.materials[i] != null)
                                {
                                    if (!AssetDatabase.Contains(explosionObject.materials[i]))
                                    {
                                        if (GUILayout.Button("Save", GUILayout.Width(48)))
                                        {
                                            #region Create Material
                                            string path = EditorUtility.SaveFilePanel("Save material", explosionCore.voxelBaseCore.GetDefaultPath(), string.Format("{0}_explosion_mat{1}.mat", explosionObject.gameObject.name, i), "mat");
                                            if (!string.IsNullOrEmpty(path))
                                            {
                                                if (path.IndexOf(Application.dataPath) < 0)
                                                {
                                                    SaveInsideAssetsFolderDisplayDialog();
                                                }
                                                else
                                                {
                                                    Undo.RecordObject(explosionObject, "Save Material");
                                                    path = path.Replace(Application.dataPath, "Assets");
                                                    AssetDatabase.CreateAsset(Material.Instantiate(explosionObject.materials[i]), path);
                                                    explosionObject.materials[i] = AssetDatabase.LoadAssetAtPath<Material>(path);
                                                }
                                            }

                                            #endregion
                                        }
                                    }
                                    {
                                        if (GUILayout.Button("Reset", GUILayout.Width(48)))
                                        {
                                            #region Reset Material
                                            Undo.RecordObject(explosionObject, "Reset Material");
                                            explosionObject.materials[i] = null;
                                            explosionCore.Generate();
                                            #endregion
                                        }
                                    }
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                    }
                    EditorGUI.indentLevel--;
                }
            }
            #endregion

            #region HelpBox
            {
                if (prefabEnable)
                {
                    List<string> helpList = new List<string>();
                    {
                        if (explosionObject.meshes == null)
                        {
                            helpList.Add("Mesh");
                        }
                        else
                        {
                            for (int i = 0; i < explosionObject.meshes.Count; i++)
                            {
                                if (explosionObject.meshes[i] == null || explosionObject.meshes[i].mesh == null || !AssetDatabase.Contains(explosionObject.meshes[i].mesh))
                                {
                                    helpList.Add("Mesh");
                                    break;
                                }
                            }
                        }
                        if (explosionObject.materials == null)
                        {
                            helpList.Add("Material");
                        }
                        else
                        {
                            for (int i = 0; i < explosionObject.materials.Count; i++)
                            {
                                if (explosionObject.materials[i] == null || !AssetDatabase.Contains(explosionObject.materials[i]))
                                {
                                    helpList.Add("Material");
                                    break;
                                }
                            }
                        }
                    }
                    if (helpList.Count > 0)
                    {
                        EditorGUILayout.HelpBox(GetPrefabHelpStrings(helpList), MessageType.Error);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.Space();
                        if (GUILayout.Button("Save All Unsaved Assets"))
                        {
                            ContextSaveAllUnsavedAssets(new MenuCommand(explosionBase));
                        }
                        EditorGUILayout.Space();
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Space();
                    }
                }
            }
            #endregion
        }

        protected override void Inspector_Bake()
        {
            explosionObject.edit_bakeFoldout = EditorGUILayout.Foldout(explosionObject.edit_bakeFoldout, "Bake", guiStyleFoldoutBold);
            if (explosionObject.edit_bakeFoldout)
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                EditorGUILayout.BeginVertical();
                {
                    var prefabType = PrefabUtility.GetPrefabType(explosionObject.gameObject);
                    var prefabEnable = prefabType == PrefabType.Prefab || prefabType == PrefabType.PrefabInstance || prefabType == PrefabType.DisconnectedPrefabInstance;

                    #region Mesh
                    {
                        if (explosionObject.meshes != null)
                        {
                            if (prefabEnable)
                            {
                                bool contains = true;
                                for (int i = 0; i < explosionObject.meshes.Count; i++)
                                {
                                    if (explosionObject.meshes[i] == null || (explosionObject.meshes[i].bakeMesh != null && !AssetDatabase.Contains(explosionObject.meshes[i].bakeMesh)))
                                        contains = false;
                                }
                                EditorGUILayout.LabelField("Mesh", contains ? EditorStyles.boldLabel : guiStyleRedBold);
                            }
                            else
                            {
                                EditorGUILayout.LabelField("Mesh", EditorStyles.boldLabel);
                            }
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Mesh", guiStyleMagentaBold);
                        }
                        {
                            EditorGUI.indentLevel++;
                            {
                                if (explosionObject.meshes != null)
                                {
                                    for (int i = 0; i < explosionObject.meshes.Count; i++)
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        {
                                            EditorGUI.BeginDisabledGroup(true);
                                            EditorGUILayout.ObjectField(explosionObject.meshes[i].bakeMesh, typeof(Mesh), false);
                                            EditorGUI.EndDisabledGroup();
                                        }
                                        if (explosionObject.meshes[i] != null && explosionObject.meshes[i].bakeMesh != null)
                                        {
                                            if (!AssetDatabase.Contains(explosionObject.meshes[i].bakeMesh))
                                            {
                                                if (GUILayout.Button("Save", GUILayout.Width(48)))
                                                {
                                                    #region Create Mesh
                                                    string path = EditorUtility.SaveFilePanel("Save mesh", explosionCore.voxelBaseCore.GetDefaultPath(), string.Format("{0}_explosion_bake_mesh{1}.asset", explosionObject.gameObject.name, i), "asset");
                                                    if (!string.IsNullOrEmpty(path))
                                                    {
                                                        if (path.IndexOf(Application.dataPath) < 0)
                                                        {
                                                            SaveInsideAssetsFolderDisplayDialog();
                                                        }
                                                        else
                                                        {
                                                            Undo.RecordObject(explosionObject, "Save Mesh");
                                                            path = path.Replace(Application.dataPath, "Assets");
                                                            AssetDatabase.CreateAsset(Mesh.Instantiate(explosionObject.meshes[i].bakeMesh), path);
                                                            explosionObject.meshes[i].bakeMesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
                                                        }
                                                    }
                                                    #endregion
                                                }
                                            }
                                            {
                                                if (GUILayout.Button("Reset", GUILayout.Width(48)))
                                                {
                                                    #region Reset Mesh
                                                    Undo.RecordObject(explosionObject, "Reset Mesh");
                                                    explosionObject.meshes[i].bakeMesh = null;
                                                    //unity bug???
                                                    explosionObject.gameObject.SetActive(!explosionObject.gameObject.activeSelf);
                                                    explosionObject.gameObject.SetActive(!explosionObject.gameObject.activeSelf);
                                                    #endregion
                                                }
                                            }
                                        }
                                        EditorGUILayout.EndHorizontal();
                                    }
                                }
                            }
                            EditorGUI.indentLevel--;
                        }
                    }
                    #endregion

                    #region HelpBox
                    {
                        if (prefabEnable)
                        {
                            List<string> helpList = new List<string>();
                            {
                                for (int i = 0; i < explosionObject.meshes.Count; i++)
                                {
                                    if (explosionObject.meshes[i] == null || (explosionObject.meshes[i].bakeMesh != null && !AssetDatabase.Contains(explosionObject.meshes[i].bakeMesh)))
                                    {
                                        helpList.Add("Mesh");
                                        break;
                                    }
                                }
                            }
                            if (helpList.Count > 0)
                            {
                                EditorGUILayout.HelpBox(GetPrefabHelpStrings(helpList), MessageType.Error);
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.Space();
                                if (GUILayout.Button("Save All Unsaved Assets"))
                                {
                                    ContextSaveAllUnsavedAssets(new MenuCommand(explosionBase));
                                }
                                EditorGUILayout.Space();
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.Space();
                            }
                        }
                    }
                    #endregion

                    EditorGUILayout.Space();

                    #region Bake
                    {
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Bake"))
                        {
                            Undo.RecordObject(explosionBase, "Bake Voxel Explosion");
                            explosionObjectCore.Bake();
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    #endregion
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
        }
        
        [MenuItem("CONTEXT/VoxelSkinnedAnimationObjectExplosion/Save All Unsaved Assets")]
        private static void ContextSaveAllUnsavedAssets(MenuCommand menuCommand)
        {
            var explosionObject = menuCommand.context as VoxelSkinnedAnimationObjectExplosion;
            if (explosionObject == null) return;

            var explosionCore = new VoxelSkinnedAnimationObjectExplosionCore(explosionObject);

            var folder = EditorUtility.OpenFolderPanel("Save all", explosionCore.voxelBaseCore.GetDefaultPath(), null);
            if (string.IsNullOrEmpty(folder)) return;
            if (folder.IndexOf(Application.dataPath) < 0)
            {
                SaveInsideAssetsFolderDisplayDialog();
                return;
            }

            Undo.RecordObject(explosionObject, "Save All Unsaved Assets");

            #region Mesh
            if (explosionObject.meshes != null)
            {
                for (int i = 0; i < explosionObject.meshes.Count; i++)
                {
                    if (explosionObject.meshes[i] != null && explosionObject.meshes[i].mesh != null && !AssetDatabase.Contains(explosionObject.meshes[i].mesh))
                    {
                        var path = folder + "/" + string.Format("{0}_explosion_mesh{1}.asset", explosionObject.gameObject.name, i);
                        path = path.Replace(Application.dataPath, "Assets");
                        AssetDatabase.CreateAsset(Mesh.Instantiate(explosionObject.meshes[i].mesh), path);
                        explosionObject.meshes[i].mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
                    }
                    if (explosionObject.meshes[i] != null && explosionObject.meshes[i].bakeMesh != null && !AssetDatabase.Contains(explosionObject.meshes[i].bakeMesh))
                    {
                        var path = folder + "/" + string.Format("{0}_explosion_bake_mesh{1}.asset", explosionObject.gameObject.name, i);
                        path = path.Replace(Application.dataPath, "Assets");
                        AssetDatabase.CreateAsset(Mesh.Instantiate(explosionObject.meshes[i].bakeMesh), path);
                        explosionObject.meshes[i].bakeMesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
                    }
                }
            }
            #endregion

            #region Material
            if (explosionObject.materials != null)
            {
                for (int index = 0; index < explosionObject.materials.Count; index++)
                {
                    if (explosionObject.materials[index] == null) continue;
                    if (AssetDatabase.Contains(explosionObject.materials[index])) continue;
                    var path = folder + "/" + string.Format("{0}_explosion_mat{1}.mat", explosionObject.gameObject.name, index);
                    path = path.Replace(Application.dataPath, "Assets");
                    AssetDatabase.CreateAsset(Material.Instantiate(explosionObject.materials[index]), path);
                    explosionObject.materials[index] = AssetDatabase.LoadAssetAtPath<Material>(path);
                }
            }
            #endregion

            InternalEditorUtility.RepaintAllViews();
        }

        [MenuItem("CONTEXT/VoxelSkinnedAnimationObjectExplosion/Reset All Assets")]
        private static void ResetAllSavedAssets(MenuCommand menuCommand)
        {
            var explosionObject = menuCommand.context as VoxelSkinnedAnimationObjectExplosion;
            if (explosionObject == null) return;

            var explosionCore = new VoxelSkinnedAnimationObjectExplosionCore(explosionObject);

            Undo.RecordObject(explosionObject, "Reset All Assets");

            #region Mesh
            explosionObject.meshes = null;
            #endregion

            #region Material
            explosionObject.materials = null;
            #endregion

            explosionCore.Generate();
            InternalEditorUtility.RepaintAllViews();
        }
    }
}

