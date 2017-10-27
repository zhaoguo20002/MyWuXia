using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SuperScrollView
{

    public class LoopListViewItem : MonoBehaviour
    {
        int mItemIndex = -1;
        int mItemId = -1;
        LoopListView mParentListView = null;
        bool mCreatedHandlerCalled = false;

        public int ItemIndex
        {
            get
            {
                return mItemIndex;
            }
            set
            {
                mItemIndex = value;
            }
        }
        public int ItemId
        {
            get
            {
                return mItemId;
            }
            set
            {
                mItemId = value;
            }
        }


        public bool CreatedHandlerCalled
        {
            get
            {
                return mCreatedHandlerCalled;
            }
            set
            {
                mCreatedHandlerCalled = value;
            }
        }

        public LoopListView ParentListView
        {
            get
            {
                return mParentListView;
            }
            set
            {
                mParentListView = value;
            }
        }

    }
}