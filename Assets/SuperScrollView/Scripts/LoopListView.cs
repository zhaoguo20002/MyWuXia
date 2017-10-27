using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public enum ListItemArrangeType
    {
        TopToBottom,
        BottomToTop,
        LeftToRight,
        RightToLeft,
    }


    public class LoopListItemGroup
    {
        int mGroupIndex = -1;
        public List<LoopListViewItem> mItemList = new List<LoopListViewItem>();
        public int GroupIndex
        {
            get
            {
                return mGroupIndex;
            }
            set
            {
                if(value == mGroupIndex)
                {
                    return;
                }
                mGroupIndex = value;
                int count = mItemList.Count;
                if(count == 1)
                {
                    mItemList[0].ItemIndex = mGroupIndex;
                    return;
                }
                int baseIndex = mGroupIndex * count;
                for(int i = 0; i< count;++i)
                {
                    mItemList[i].ItemIndex = baseIndex + i;
                }
            }


        }
    }



    public class ItemPoolMgr
    {
        static int mItemCreatedCount = 0;
        public GameObject mItemPrefab;
        List<LoopListViewItem> mPoolItemList = new List<LoopListViewItem>();
        public LoopListViewItem GetItem()
        {
            int count = mPoolItemList.Count;
            if (count > 0)
            {
                LoopListViewItem ret = mPoolItemList[count - 1];
                mPoolItemList.RemoveAt(count - 1);
                ret.gameObject.SetActive(true);
                return ret;
            }
            GameObject obj = GameObject.Instantiate<GameObject>(mItemPrefab);
            obj.SetActive(true);
            obj.name = mItemCreatedCount.ToString();
            LoopListViewItem tViewItem = obj.GetComponent<LoopListViewItem>();
            if (tViewItem == null)
            {
                tViewItem = obj.AddComponent<LoopListViewItem>();
            }
            tViewItem.ItemId = mItemCreatedCount;
            mItemCreatedCount++;
            return tViewItem;
        }

        public void RecycleItem(LoopListViewItem item)
        {
            item.gameObject.SetActive(false);
            mPoolItemList.Add(item);
        }

        public void Init(GameObject itemPrefab)
        {
            mItemPrefab = itemPrefab;
            mItemCreatedCount = 0;
        }
    }

    public class LoopListView : MonoBehaviour
    {

        [SerializeField]
        private GameObject mItemPrefab;
        public GameObject ItemPrefab { get { return mItemPrefab; } set { mItemPrefab = value; } }

        [SerializeField]
        private Vector2 mItemSize;
        public Vector2 ItemSize { get { return mItemSize; } set { mItemSize = value; } }

        [SerializeField]
        private int mItemCountPerGroup = 1;
        public int ItemCountPerGroup { get { return mItemCountPerGroup; } set { mItemCountPerGroup = value; } }

        [SerializeField]
        private Vector2 mPadding;
        public Vector2 Padding { get { return mPadding; } set { mPadding = value; } }


        [SerializeField]
        private ListItemArrangeType mArrangeType = ListItemArrangeType.TopToBottom;
        public ListItemArrangeType ArrangeType { get { return mArrangeType; } set { mArrangeType = value; } }


        ItemPoolMgr mItemPoolMgr = new ItemPoolMgr();
        List<LoopListItemGroup> mItemGroupList = new List<LoopListItemGroup>();
        RectTransform mContainerTrans;
        ScrollRect mScrollRect = null;
        int mItemTotalCount = 0;
        int mCreatedItemGroupCount = 0;
        int mLastHeadGroupIndex = -1;
        int mTotalItemGroupCount = 0;
        float mLastPosX = -1;
        float mLastPosY = -1;
        Vector2 mContentSize;
        Vector2 mMaxLocalPosition;
        bool mIsVertList = false;
        int mMoreCreateValue = 2;
        float mItemPaddingHeight;
        float mItemPaddingWidth;
        Vector2 mListViewPortSize;
        System.Action<LoopListViewItem> mOnItemCreated;
        System.Action<LoopListViewItem> mOnItemIndexUpdated;
        bool mNeedLoopItem = false;

        public int ItemTotalCount
        {
            get
            {
                return mItemTotalCount;
            }
        }


        /*
        itemTotalCount: the total item count in the listview.
        onItemCreated: when a new item is created, this Action will be called.
        onItemIndexUpdated: when a item’s index value is changed, this Action will be called.
        */
        public void InitListView(int itemTotalCount, System.Action<LoopListViewItem> onItemCreated, System.Action<LoopListViewItem> onItemIndexUpdated)
        {
            if(itemTotalCount < 0)
            {
                itemTotalCount = 0;
            }
            if(mItemPrefab == null)
            {
                Debug.LogError("ItemPrefab is empty value!");
                return;
            }
            mItemPoolMgr.Init(mItemPrefab);
            mScrollRect = gameObject.GetComponent<ScrollRect>();
            if(mScrollRect == null)
            {
                Debug.LogError("ScrollRect component not found!");
                return;
            }
            mContainerTrans = mScrollRect.content;
            mOnItemCreated = onItemCreated;
            mOnItemIndexUpdated = onItemIndexUpdated;
            mScrollRect.onValueChanged.AddListener(delegate { OnSrollRectValueChanged(); });
            SetListItemCount(itemTotalCount);
        }

        /*
        to set the item total count of the listview at runtime.
        If resetPos is set false, then the scrollrect’s content position will not changed after this method finished.
         */
        public void SetListItemCount(int itemCount, bool resetPos = true)
        {
            if(itemCount < 0)
            {
                itemCount = 0;
            }
            Vector3 pos = mContainerTrans.localPosition;
            mItemTotalCount = itemCount;
            ReflushListViewContent();
            if (resetPos == false)
            {
                MovePanelToPosition(pos.x, pos.y);
                ReflushAllCreatedGroup();
            }
        }

        public void ResetPanelPos()
        {
            MovePanelByScrollValue(0);
        }

        /*
        when the scrollrect is horizontal, this method will move the scrollrect content’s localposition.x to the indicated x.
        when the scrollrect is vertical, this method will move the scrollrect content’s localposition.y to the indicated y.
         */
        public void MovePanelToPosition(float x, float y)
        {
            mScrollRect.StopMovement();
            Vector3 vt = mContainerTrans.localPosition;
            if (mArrangeType == ListItemArrangeType.TopToBottom)
            {
                vt.y = Mathf.Clamp(y, 0, mMaxLocalPosition.y);
            }
            else if (mArrangeType == ListItemArrangeType.BottomToTop)
            {
                vt.y = Mathf.Clamp(y, -mMaxLocalPosition.y, 0);
            }
            else if (mArrangeType == ListItemArrangeType.LeftToRight)
            {
                vt.x = Mathf.Clamp(x, -mMaxLocalPosition.x, 0);
            }
            else if (mArrangeType == ListItemArrangeType.RightToLeft)
            {
                vt.x = Mathf.Clamp(x, 0, mMaxLocalPosition.x);
            }
            mContainerTrans.localPosition = vt;

        }


        /*
        move the scrollrect content’s position to the indicated value. 
        normalizedVal is from 0 to 1.
        For a TopToBottom arranged scrollrect,normalizedVal 0 means the downmost point of the list can reach, and normalizedVal 1 means the topmost point of the list can reach.
        For a BottomToTop arranged scrollrect, normalizedVal 0 means the topmost point of the list can reach, and normalizedVal 1 means the downmost point of the list can reach.
         */
        public void MovePanelByScrollNormalizedValue(float normalizedVal)
        {
            float val = 0;
            if (mIsVertList)
            {
                val = mMaxLocalPosition.y * normalizedVal;
            }
            else
            {
                val = mMaxLocalPosition.x * normalizedVal;
            }
            MovePanelByScrollValue(val);
        }


        /*
        val is indicating how much distance the scrollrect scrolled.
        when the scrollrect is horizontal, val is from 0 to Mathf.Abs(Content.size.x – ViewPort.size.x), this method will move the scrollrect content’s localposition.x to the indicated value.
        when the scrollrect is vertical, val is from 0 to Mathf.Abs(Content.size.y – ViewPort.size.y), this method will move the scrollrect content’s localposition.y to the indicated value.
        */
        public void MovePanelByScrollValue(float val)
        {
            mScrollRect.StopMovement();
            if (val < 0)
            {
                val = 0;
            }
            Vector3 vt = mContainerTrans.localPosition;
            if (mArrangeType == ListItemArrangeType.TopToBottom)
            {
                vt.y = Mathf.Clamp(val, 0, mMaxLocalPosition.y);
            }
            else if (mArrangeType == ListItemArrangeType.BottomToTop)
            {
                vt.y = Mathf.Clamp(-val, -mMaxLocalPosition.y, 0);
            }
            else if (mArrangeType == ListItemArrangeType.LeftToRight)
            {
                vt.x = Mathf.Clamp(-val, -mMaxLocalPosition.x,0);
            }
            else if (mArrangeType == ListItemArrangeType.RightToLeft)
            {
                vt.x = Mathf.Clamp(val, 0,mMaxLocalPosition.x);
            }
            mContainerTrans.localPosition = vt;
        }



        public int ItemIndexToGroupIndex(int itemIndex)
        {
            int groupIndex = itemIndex / mItemCountPerGroup;
            return groupIndex;
        }


        /*
         itemIndex starts at 0.
         move the scrollrect content's position to ( the positon of itemIndex-th item + offset )  
         */
        public void MovePanelToItemIndex(int itemIndex, float offset = 0)
        {
            int groupIndex = ItemIndexToGroupIndex(itemIndex);
            MovePanelToGroupIndex(groupIndex, offset);
        }


        /*
        groupIndex starts at 0.
        when the scrollrect is horizontal, This method will move the scrollrect content’s localposition.x to ( the position of groupIndex-th column + offset )
        when the scrollrect is vertical, This method will move the scrollrect content’s localposition.y to ( the position of groupIndex-th row + offset )
        */
        public void MovePanelToGroupIndex(int groupIndex, float offset = 0)
        {
            float val = 0;
            if (mIsVertList)
            {
                val = mItemPaddingHeight * groupIndex + offset;
            }
            else
            {
                val = mItemPaddingWidth * groupIndex + offset;
            }
            MovePanelByScrollValue(val);
        }


        public void ReflushListViewContent()
        {
            RecycleAllGroupItem();
            if(mScrollRect.horizontalScrollbarVisibility == ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport)
            {
                mScrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
            }
            if(mScrollRect.verticalScrollbarVisibility == ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport)
            {
                mScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
            }
            mIsVertList = (mArrangeType == ListItemArrangeType.TopToBottom || mArrangeType == ListItemArrangeType.BottomToTop);
            mScrollRect.horizontal = !mIsVertList;
            mScrollRect.vertical = mIsVertList;
            mListViewPortSize = mScrollRect.viewport.rect.size;
            mItemPaddingHeight = mItemSize.y + mPadding.y;
            mItemPaddingWidth = mItemSize.x + mPadding.x;
            if (mIsVertList)
            {
                if (mArrangeType == ListItemArrangeType.TopToBottom)
                {
                    mScrollRect.viewport.pivot = new Vector2(0, 1);
                }
                else
                {
                    mScrollRect.viewport.pivot = new Vector2(0, 0);
                }
                mCreatedItemGroupCount = (Mathf.CeilToInt(mListViewPortSize.y / (mItemSize.y + mPadding.y)) + mMoreCreateValue);
            }
            else
            {
                if (mArrangeType == ListItemArrangeType.LeftToRight)
                {
                    mScrollRect.viewport.pivot = new Vector2(0, 1);
                }
                else
                {
                    mScrollRect.viewport.pivot = new Vector2(1, 1);
                }
                mCreatedItemGroupCount = (Mathf.CeilToInt(mListViewPortSize.x / (mItemSize.x + mPadding.x)) + mMoreCreateValue);
            }
            mItemPrefab.GetComponent<RectTransform>().pivot = mScrollRect.viewport.pivot;
            mContainerTrans.pivot = mScrollRect.viewport.pivot;
            mTotalItemGroupCount = mItemTotalCount / mItemCountPerGroup;
            if (mItemTotalCount % mItemCountPerGroup > 0)
            {
                mTotalItemGroupCount++;
            }
            if (mCreatedItemGroupCount >= mTotalItemGroupCount)
            {
                mNeedLoopItem = false;
                mCreatedItemGroupCount = mTotalItemGroupCount;
            }
            else
            {
                mNeedLoopItem = true;
            }
            mLastHeadGroupIndex = -1;
            mItemGroupList.Clear();
            UpdateContentSize();
            mContentSize = mContainerTrans.rect.size;
            mMaxLocalPosition = mContentSize - mListViewPortSize;
            if(mMaxLocalPosition.x < 0 )
            {
                mMaxLocalPosition.x = 0;
            }
            if(mMaxLocalPosition.y < 0)
            {
                mMaxLocalPosition.y = 0;
            }
            for (int i = 0; i < mCreatedItemGroupCount; ++i)
            {
                CreateItemGroup(i);
            }
            mItemPrefab.SetActive(false);
            foreach (LoopListItemGroup group in mItemGroupList)
            {
                OnGroupCreated(group);
            }
            ResetPanelPos();
            ReflushAllCreatedGroup();
        }


        public void ReflushAllCreatedGroup()
        {
            int curIndex = 0;
            if (mArrangeType == ListItemArrangeType.TopToBottom)
            {
                float curY = Mathf.Clamp(mContainerTrans.localPosition.y, 0, mMaxLocalPosition.y);
                curIndex = Mathf.FloorToInt(curY / mItemPaddingHeight);
            }
            else if (mArrangeType == ListItemArrangeType.BottomToTop)
            {
                float curY = Mathf.Clamp(-mContainerTrans.localPosition.y, 0, mMaxLocalPosition.y);
                curIndex = Mathf.FloorToInt(curY / mItemPaddingHeight);
            }
            else if (mArrangeType == ListItemArrangeType.RightToLeft)
            {
                float curX = Mathf.Clamp(mContainerTrans.localPosition.x, 0, mMaxLocalPosition.x);
                curIndex = Mathf.FloorToInt(curX / mItemPaddingWidth);
            }
            else if (mArrangeType == ListItemArrangeType.LeftToRight)
            {
                float curX = Mathf.Clamp(-mContainerTrans.localPosition.x, 0, mMaxLocalPosition.x);
                curIndex = Mathf.FloorToInt(curX / mItemPaddingWidth);
            }
            int count = mItemGroupList.Count;
            for (int i = 0; i < count; ++i)
            {
                LoopListItemGroup it = mItemGroupList[i];
                it.GroupIndex = i + curIndex;
                ResetGroupItemPos(it);
                UpdateGroupItems(it);
            }
        }


        void RecycleAllGroupItem()
        {
            foreach (LoopListItemGroup group in mItemGroupList)
            {
                RecycleGroupItem(group);
            }
            mItemGroupList.Clear();
        }

        void RecycleGroupItem(LoopListItemGroup group)
        {
            foreach (LoopListViewItem item in group.mItemList)
            {
                mItemPoolMgr.RecycleItem(item);
            }
            group.mItemList.Clear();
        }



        void OnGroupCreated(LoopListItemGroup group)
        {
            if (mOnItemCreated == null)
            {
                return;
            }
            int count = group.mItemList.Count;
            for (int i = 0; i < count; ++i)
            {
                LoopListViewItem tItem = group.mItemList[i];
                if(tItem.CreatedHandlerCalled == false)
                {
                    tItem.CreatedHandlerCalled = true;
                    mOnItemCreated(tItem);
                }
            }
        }



        void UpdateGroupItems(LoopListItemGroup group)
        {
            if (mOnItemIndexUpdated == null)
            {
                return;
            }
            int count = group.mItemList.Count;
            for (int i = 0; i < count; ++i)
            {
                LoopListViewItem tViewItem = group.mItemList[i];
                if(tViewItem.ItemIndex >= mItemTotalCount)
                {
                    tViewItem.gameObject.SetActive(false);
                }
                else
                {
                    tViewItem.gameObject.SetActive(true);
                    mOnItemIndexUpdated(tViewItem);
                }
            }
        }

        void CreateItemGroup(int groupIndex)
        {
            LoopListItemGroup tGroup = new LoopListItemGroup();
            mItemGroupList.Add(tGroup);
            for (int i = 0; i < mItemCountPerGroup;++i)
            {
                LoopListViewItem tViewItem = mItemPoolMgr.GetItem();
                RectTransform rf = tViewItem.GetComponent<RectTransform>();
                rf.SetParent(mContainerTrans);
                rf.localScale = Vector3.one;
                rf.localPosition = Vector3.zero;
                rf.rotation = Quaternion.identity;
                tViewItem.ParentListView = this;
                tGroup.mItemList.Add(tViewItem);
            }
            tGroup.GroupIndex = groupIndex;
        }


        void ResetGroupItemPos(LoopListItemGroup group)
        {
            int index = group.GroupIndex;
            int count = group.mItemList.Count;
            if (mIsVertList)
            {
                float y = (mArrangeType == ListItemArrangeType.TopToBottom)?(-index * mItemPaddingHeight) : (index * mItemPaddingHeight);
                for (int i = 0; i < count; ++i)
                {
                    group.mItemList[i].transform.localPosition = new Vector3(i* mItemPaddingWidth, y, 0.0f);
                }
            }
            else
            {
                float x = (mArrangeType == ListItemArrangeType.LeftToRight)?(index * mItemPaddingWidth) :(-index * mItemPaddingWidth);
                for (int i = 0; i < count; ++i)
                {
                    group.mItemList[i].transform.localPosition = new Vector3(x, -i * mItemPaddingHeight, 0.0f);
                }
            }
        }


        void OnSrollRectValueChanged()
        {
            if(mNeedLoopItem == false)
            {
                return;
            }
            if(mIsVertList)
            {
                UpdateForVertList();
            }
            else
            {
                UpdateForHorizonList();
            }
        }


        void UpdateForHorizonList()
        {
            bool isRightScroll = mContainerTrans.localPosition.x > mLastPosX;
            mLastPosX = mContainerTrans.localPosition.x;
            if (mArrangeType == ListItemArrangeType.RightToLeft)
            {
                float curX = Mathf.Clamp(mContainerTrans.localPosition.x, 0, mMaxLocalPosition.x);
                int curIndex = Mathf.FloorToInt(curX / mItemPaddingWidth);
                if (mLastHeadGroupIndex == curIndex)
                {
                    return;
                }
                mLastHeadGroupIndex = curIndex;
                int count = mItemGroupList.Count;
                int realHeadIndex = curIndex;
                if (isRightScroll)
                {
                    if (mItemGroupList[mItemGroupList.Count - 1].GroupIndex == mTotalItemGroupCount - 1)
                    {
                        return;
                    }
                    int indexDelta = realHeadIndex - mItemGroupList[0].GroupIndex;
                    if (indexDelta == 0)
                    {
                        return;
                    }
                    if (indexDelta > 0 && indexDelta <= mMoreCreateValue)
                    {
                        for (int i = 1; i < count; ++i)
                        {
                            LoopListItemGroup it = mItemGroupList[i];
                            if (it.GroupIndex != i + realHeadIndex - indexDelta)
                            {
                                ReflushAllCreatedGroup(realHeadIndex);
                                return;
                            }
                        }
                        for (int i = 1; i <= indexDelta; ++i)
                        {
                            LoopListItemGroup it0 = mItemGroupList[0];
                            if (it0.GroupIndex + count >= mTotalItemGroupCount)
                            {
                                break;
                            }
                            it0.GroupIndex = it0.GroupIndex + count;
                            mItemGroupList.RemoveAt(0);
                            mItemGroupList.Add(it0);
                            ResetGroupItemPos(it0);
                            UpdateGroupItems(it0);
                        }
                    }
                    else
                    {
                        ReflushAllCreatedGroup(realHeadIndex);
                    }
                }
                else
                {
                    if (mItemGroupList[0].GroupIndex == 0)
                    {
                        return;
                    }
                    float dist = mContentSize.x - mListViewPortSize.x - curX;
                    int index2 = Mathf.FloorToInt(dist / mItemPaddingWidth);
                    int realTailIndex = mTotalItemGroupCount - index2 - 1;
                    int indexDelta = mItemGroupList[count - 1].GroupIndex - realTailIndex;
                    if (indexDelta == 0)
                    {
                        return;
                    }
                    if (indexDelta > 0 && indexDelta <= mMoreCreateValue)
                    {
                        for (int i = 1; i < count; ++i)
                        {
                            LoopListItemGroup it = mItemGroupList[count - i - 1];
                            if (it.GroupIndex != realTailIndex + indexDelta - i)
                            {
                                ReflushAllCreatedGroup(realTailIndex - count + 1);
                                return;
                            }
                        }

                        for (int i = 1; i <= indexDelta; ++i)
                        {
                            LoopListItemGroup it0 = mItemGroupList[count - 1];
                            if (it0.GroupIndex - count < 0)
                            {
                                break;
                            }
                            it0.GroupIndex = it0.GroupIndex - count;
                            mItemGroupList.RemoveAt(count - 1);
                            mItemGroupList.Insert(0, it0);
                            ResetGroupItemPos(it0);
                            UpdateGroupItems(it0);
                        }
                    }
                    else
                    {
                        ReflushAllCreatedGroup(realTailIndex - count + 1);
                    }
                }

            }
            else
            {
                float curX = Mathf.Clamp(-mContainerTrans.localPosition.x, 0, mMaxLocalPosition.x);
                int curIndex = Mathf.FloorToInt(curX / mItemPaddingWidth);
                if (mLastHeadGroupIndex == curIndex)
                {
                    return;
                }
                mLastHeadGroupIndex = curIndex;
                int count = mItemGroupList.Count;
                int realHeadIndex = curIndex;
                if (!isRightScroll)
                {
                    if (mItemGroupList[mItemGroupList.Count - 1].GroupIndex == mTotalItemGroupCount - 1)
                    {
                        return;
                    }
                    int indexDelta = realHeadIndex - mItemGroupList[0].GroupIndex;
                    if (indexDelta == 0)
                    {
                        return;
                    }
                    if (indexDelta > 0 && indexDelta <= mMoreCreateValue)
                    {
                        for (int i = 1; i < count; ++i)
                        {
                            LoopListItemGroup it = mItemGroupList[i];
                            if (it.GroupIndex != i + realHeadIndex - indexDelta)
                            {
                                ReflushAllCreatedGroup(realHeadIndex);
                                return;
                            }
                        }
                        for (int i = 1; i <= indexDelta; ++i)
                        {
                            LoopListItemGroup it0 = mItemGroupList[0];
                            if (it0.GroupIndex + count >= mTotalItemGroupCount)
                            {
                                break;
                            }
                            it0.GroupIndex = it0.GroupIndex + count;
                            mItemGroupList.RemoveAt(0);
                            mItemGroupList.Add(it0);
                            ResetGroupItemPos(it0);
                            UpdateGroupItems(it0);
                        }
                    }
                    else
                    {
                        ReflushAllCreatedGroup(realHeadIndex);
                    }
                }
                else
                {
                    if (mItemGroupList[0].GroupIndex == 0)
                    {
                        return;
                    }
                    float dist = mContentSize.x - mListViewPortSize.x - curX;
                    int index2 = Mathf.FloorToInt(dist / mItemPaddingWidth);
                    int realTailIndex = mTotalItemGroupCount - index2 - 1;
                    int indexDelta = mItemGroupList[count - 1].GroupIndex - realTailIndex;
                    if (indexDelta == 0)
                    {
                        return;
                    }
                    if (indexDelta > 0 && indexDelta <= mMoreCreateValue)
                    {
                        for (int i = 1; i < count; ++i)
                        {
                            LoopListItemGroup it = mItemGroupList[count - i - 1];
                            if (it.GroupIndex != realTailIndex + indexDelta - i)
                            {
                                ReflushAllCreatedGroup(realTailIndex - count + 1);
                                return;
                            }
                        }

                        for (int i = 1; i <= indexDelta; ++i)
                        {
                            LoopListItemGroup it0 = mItemGroupList[count - 1];
                            if (it0.GroupIndex - count < 0)
                            {
                                break;
                            }
                            it0.GroupIndex = it0.GroupIndex - count;
                            mItemGroupList.RemoveAt(count - 1);
                            mItemGroupList.Insert(0, it0);
                            ResetGroupItemPos(it0);
                            UpdateGroupItems(it0);
                        }
                    }
                    else
                    {
                        ReflushAllCreatedGroup(realTailIndex - count + 1);
                    }
                }
            }

        }


        void UpdateForVertList()
        {
            bool isUpScroll = mContainerTrans.localPosition.y > mLastPosY;
            mLastPosY = mContainerTrans.localPosition.y;
            if(mArrangeType == ListItemArrangeType.TopToBottom)
            {
                float curY = Mathf.Clamp(mContainerTrans.localPosition.y, 0, mMaxLocalPosition.y);
                int curIndex = Mathf.FloorToInt(curY / mItemPaddingHeight);
                if (mLastHeadGroupIndex == curIndex)
                {
                    return;
                }
                mLastHeadGroupIndex = curIndex;
                int count = mItemGroupList.Count;
                int realHeadIndex = curIndex;
                if (isUpScroll)
                {
                    if(mItemGroupList[mItemGroupList.Count -1].GroupIndex == mTotalItemGroupCount-1)
                    {
                        return;
                    }
                    int indexDelta = realHeadIndex - mItemGroupList[0].GroupIndex;
                    if (indexDelta == 0)
                    {
                        return;
                    }
                    if (indexDelta > 0 && indexDelta <= mMoreCreateValue)
                    {
                        for (int i = 1; i < count; ++i)
                        {
                            LoopListItemGroup it = mItemGroupList[i];
                            if (it.GroupIndex != i + realHeadIndex - indexDelta)
                            {
                                ReflushAllCreatedGroup(realHeadIndex);
                                return;
                            }
                        }
                        for (int i = 1; i <= indexDelta; ++i)
                        {
                            LoopListItemGroup it0 = mItemGroupList[0];
                            if(it0.GroupIndex + count >= mTotalItemGroupCount)
                            {
                                break;
                            }
                            it0.GroupIndex = it0.GroupIndex + count;
                            mItemGroupList.RemoveAt(0);
                            mItemGroupList.Add(it0);
                            ResetGroupItemPos(it0);
                            UpdateGroupItems(it0);
                        }
                    }
                    else
                    {
                        ReflushAllCreatedGroup(realHeadIndex);
                    }
                }
                else
                {
                    if (mItemGroupList[0].GroupIndex == 0)
                    {
                        return;
                    }
                    float dist = mContentSize.y - mListViewPortSize.y - curY;
                    int index2 = Mathf.FloorToInt(dist / mItemPaddingHeight);
                    int realTailIndex = mTotalItemGroupCount - index2 - 1;
                    int indexDelta = mItemGroupList[count - 1].GroupIndex - realTailIndex;
                    if (indexDelta == 0)
                    {
                        return;
                    }
                    if (indexDelta > 0 && indexDelta <= mMoreCreateValue)
                    {
                        for (int i = 1; i < count; ++i)
                        {
                            LoopListItemGroup it = mItemGroupList[count - i - 1];
                            if (it.GroupIndex != realTailIndex + indexDelta - i)
                            {
                                ReflushAllCreatedGroup(realTailIndex - count + 1);
                                return;
                            }
                        }

                        for (int i = 1; i <= indexDelta; ++i)
                        {
                            LoopListItemGroup it0 = mItemGroupList[count - 1];
                            if(it0.GroupIndex - count < 0)
                            {
                                break;
                            }
                            it0.GroupIndex = it0.GroupIndex - count;
                            mItemGroupList.RemoveAt(count - 1);
                            mItemGroupList.Insert(0, it0);
                            ResetGroupItemPos(it0);
                            UpdateGroupItems(it0);
                        }
                    }
                    else
                    {
                        ReflushAllCreatedGroup(realTailIndex - count + 1);
                    }
                }

            }
            else
            {
                float curY = Mathf.Clamp(-mContainerTrans.localPosition.y, 0, mMaxLocalPosition.y);
                int curIndex = Mathf.FloorToInt(curY / mItemPaddingHeight);
                if (mLastHeadGroupIndex == curIndex)
                {
                    return;
                }
                mLastHeadGroupIndex = curIndex;
                int count = mItemGroupList.Count;
                int realHeadIndex = curIndex;
                if (!isUpScroll)
                {
                    if (mItemGroupList[mItemGroupList.Count - 1].GroupIndex == mTotalItemGroupCount - 1)
                    {
                        return;
                    }
                    int indexDelta = realHeadIndex - mItemGroupList[0].GroupIndex;
                    if (indexDelta == 0)
                    {
                        return;
                    }
                    if (indexDelta > 0 && indexDelta <= mMoreCreateValue)
                    {
                        for (int i = 1; i < count; ++i)
                        {
                            LoopListItemGroup it = mItemGroupList[i];
                            if (it.GroupIndex != i + realHeadIndex - indexDelta)
                            {
                                ReflushAllCreatedGroup(realHeadIndex);
                                return;
                            }
                        }
                        for (int i = 1; i <= indexDelta; ++i)
                        {
                            LoopListItemGroup it0 = mItemGroupList[0];
                            if (it0.GroupIndex + count >= mTotalItemGroupCount)
                            {
                                break;
                            }
                            it0.GroupIndex = it0.GroupIndex + count;
                            mItemGroupList.RemoveAt(0);
                            mItemGroupList.Add(it0);
                            ResetGroupItemPos(it0);
                            UpdateGroupItems(it0);
                        }
                    }
                    else
                    {
                        ReflushAllCreatedGroup(realHeadIndex);
                    }
                }
                else
                {
                    if (mItemGroupList[0].GroupIndex == 0)
                    {
                        return;
                    }
                    float dist = mContentSize.y - mListViewPortSize.y - curY;
                    int index2 = Mathf.FloorToInt(dist / mItemPaddingHeight);
                    int realTailIndex = mTotalItemGroupCount - index2 - 1;
                    int indexDelta = mItemGroupList[count - 1].GroupIndex - realTailIndex;
                    if (indexDelta == 0)
                    {
                        return;
                    }
                    if (indexDelta > 0 && indexDelta <= mMoreCreateValue)
                    {
                        for (int i = 1; i < count; ++i)
                        {
                            LoopListItemGroup it = mItemGroupList[count - i - 1];
                            if (it.GroupIndex != realTailIndex + indexDelta - i)
                            {
                                ReflushAllCreatedGroup(realTailIndex - count + 1);
                                return;
                            }
                        }

                        for (int i = 1; i <= indexDelta; ++i)
                        {
                            LoopListItemGroup it0 = mItemGroupList[count - 1];
                            if (it0.GroupIndex - count < 0)
                            {
                                break;
                            }
                            it0.GroupIndex = it0.GroupIndex - count;
                            mItemGroupList.RemoveAt(count - 1);
                            mItemGroupList.Insert(0, it0);
                            ResetGroupItemPos(it0);
                            UpdateGroupItems(it0);
                        }
                    }
                    else
                    {
                        ReflushAllCreatedGroup(realTailIndex - count + 1);
                    }
                }
            }
           
        }


        void ReflushAllCreatedGroup(int realHeadIndex)
        {
            if(realHeadIndex < 0 )
            {
                realHeadIndex = 0;
            }
            int count = mItemGroupList.Count;
            for (int i = 0; i < count; ++i)
            {
                LoopListItemGroup it = mItemGroupList[i];
                it.GroupIndex = i + realHeadIndex;
                ResetGroupItemPos(it);
                UpdateGroupItems(it);
            }
        }


        void UpdateContentSize()
        {
            if (mIsVertList)
            {
                mContainerTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mTotalItemGroupCount * mItemPaddingHeight - mPadding.y);
            }
            else
            {
                mContainerTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mTotalItemGroupCount * mItemPaddingWidth - mPadding.x);
            }
        }
    }

}
