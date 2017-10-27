using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{

    public class DemoSceneScript : MonoBehaviour
    {
        public Button mNextPageButton;
        public Button mPrevPageButton;
        public Button mJumpToButton;
        public Button mAddItemButton;
        public Button mSetItemCountButton;
        public InputField mJumpInput;
        public InputField mAddItemInput;
        public InputField mItemCountInput;
        public Text mTitle;
        void Start()
        {
            mNextPageButton.onClick.AddListener(OnNextButtonClicked);
            mPrevPageButton.onClick.AddListener(OnPrevButtonClicked);
            mJumpToButton.onClick.AddListener(OnJumpBtnClicked);
            mAddItemButton.onClick.AddListener(OnAddItemBtnClicked);
            mSetItemCountButton.onClick.AddListener(OnSetItemCountBtnClicked);
            int pageCount = PageManager.Instance.PageCount;
            for(int i = 0;i< pageCount;++i)
            {
                PageData tPageData = PageManager.Instance.GetPageData(i);
                tPageData.mLoopListView.InitListView(tPageData.mItemCount, OnItemCreated, OnItemUpdated);
            }
            PageManager.Instance.ResetData();
            UpdateView();
        }

        void OnItemCreated(LoopListViewItem item)
        {
            ListItem1 itemScript = item.GetComponent<ListItem1>();
            itemScript.mBtn.onClick.AddListener(delegate () {
                this.OnItemBtnClicked(item);
            });
            
        }

        void OnItemUpdated(LoopListViewItem item)
        {
            ListItem1 itemScript = item.GetComponent<ListItem1>();
            itemScript.mBtnText.text = item.ItemIndex.ToString();
            int count = ResManager.Instance.SpriteCount;
            int spriteIndex = (itemScript.mSpriteStartIndex + item.ItemIndex % 18) % count;
            itemScript.mItemIcon.sprite = ResManager.Instance.GetSpriteByIndex(spriteIndex);
        }

        void OnItemBtnClicked(LoopListViewItem item)
        {
            Debug.Log("Item Btn Clicked. Item Index is "+item.ItemIndex+" item Id is "+item.ItemId);
        }

        void UpdateView()
        {
            UpdatePageButtonStatus();
            PageData tPageData = PageManager.Instance.GetCurShowingPageData();
            if(tPageData == null)
            {
                return;
            }
            mTitle.text = tPageData.mTitle;
        }


        void UpdatePageButtonStatus()
        {
            if (PageManager.Instance.PageCount<= 1)
            {
                mPrevPageButton.gameObject.SetActive(false);
                mNextPageButton.gameObject.SetActive(false);
                return;
            }
            if (PageManager.Instance.IsShowingFirstPage())
            {
                mPrevPageButton.gameObject.SetActive(false);
                mNextPageButton.gameObject.SetActive(true);
            }
            else if (PageManager.Instance.IsShowingLastPage())
            {
                mPrevPageButton.gameObject.SetActive(true);
                mNextPageButton.gameObject.SetActive(false);
            }
            else
            {
                mPrevPageButton.gameObject.SetActive(true);
                mNextPageButton.gameObject.SetActive(true);
            }
        }


        public void OnPrevButtonClicked()
        {
            if (PageManager.Instance.IsShowingFirstPage())
            {
                return;
            }
            int curShowingPage = PageManager.Instance.CurShowingPageIndex;
            PageManager.Instance.ShowPanel(curShowingPage - 1);
            UpdateView();
        }

        public void OnNextButtonClicked()
        {
            if (PageManager.Instance.IsShowingLastPage())
            {
                return;
            }
            int curShowingPage = PageManager.Instance.CurShowingPageIndex;
            PageManager.Instance.ShowPanel(curShowingPage + 1);
            UpdateView();
        }

        void OnJumpBtnClicked()
        {
            PageData tPageData = PageManager.Instance.GetCurShowingPageData();
            if (tPageData == null)
            {
                return;
            }
            int itemIndex = 0;
            if(int.TryParse(mJumpInput.text,out itemIndex) == false)
            {
                return;
            }
            tPageData.mLoopListView.MovePanelToItemIndex(itemIndex, 0);
        }

        void OnAddItemBtnClicked()
        {
            PageData tPageData = PageManager.Instance.GetCurShowingPageData();
            if (tPageData == null)
            {
                return;
            }
            int addCount = 0;
            if (int.TryParse(mAddItemInput.text, out addCount) == false)
            {
                mAddItemInput.text = "0";
                return;
            }
            int curListItemCount = tPageData.mLoopListView.ItemTotalCount;
            tPageData.mLoopListView.SetListItemCount(curListItemCount + addCount,false);
        }

        void OnSetItemCountBtnClicked()
        {
            PageData tPageData = PageManager.Instance.GetCurShowingPageData();
            if (tPageData == null)
            {
                return;
            }
            int itemCount = 0;
            if (int.TryParse(mItemCountInput.text, out itemCount) == false)
            {
                return;
            }
            tPageData.mLoopListView.SetListItemCount(itemCount);
        }


    }
}
