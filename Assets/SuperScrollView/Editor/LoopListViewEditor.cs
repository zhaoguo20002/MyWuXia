using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditorInternal;


namespace SuperScrollView
{

    [CustomEditor(typeof(LoopListView))]
    public class LoopListViewEditor : Editor
    {

        SerializedProperty mItemPrefab;
        SerializedProperty mItemSize;
        SerializedProperty mItemCountPerGroup;
        SerializedProperty mPadding;
        SerializedProperty mArrangeType;

        GUIContent mItemPrefabGuiContent = new GUIContent("ItemPrefab");
        GUIContent mItemSizeGuiContent = new GUIContent("ItemSize");
        GUIContent mItemCountPerGroupGuiContent1 = new GUIContent("ItemCountPerRow");
        GUIContent mItemCountPerGroupGuiContent2 = new GUIContent("ItemCountPerColumn");
        GUIContent mPaddingGuiContent = new GUIContent("ItemPadding");
        GUIContent mArrangeTypeGuiContent = new GUIContent("ArrangeType");



        protected virtual void OnEnable()
        {
            mItemPrefab = serializedObject.FindProperty("mItemPrefab");
            mItemSize = serializedObject.FindProperty("mItemSize");
            mItemCountPerGroup = serializedObject.FindProperty("mItemCountPerGroup");
            mPadding = serializedObject.FindProperty("mPadding");
            mArrangeType = serializedObject.FindProperty("mArrangeType");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            LoopListView tListView = serializedObject.targetObject as LoopListView;
            if(tListView == null)
            {
                return;
            }
            EditorGUILayout.PropertyField(mItemPrefab,mItemPrefabGuiContent);
            EditorGUILayout.PropertyField(mItemSize, mItemSizeGuiContent);
            if(tListView.ArrangeType == ListItemArrangeType.BottomToTop || tListView.ArrangeType == ListItemArrangeType.TopToBottom)
            {
                EditorGUILayout.PropertyField(mItemCountPerGroup, mItemCountPerGroupGuiContent1);
            }
            else
            {
                EditorGUILayout.PropertyField(mItemCountPerGroup, mItemCountPerGroupGuiContent2);
            }
            EditorGUILayout.PropertyField(mPadding, mPaddingGuiContent);
            EditorGUILayout.PropertyField(mArrangeType, mArrangeTypeGuiContent);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
