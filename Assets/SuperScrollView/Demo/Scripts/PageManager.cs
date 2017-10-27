using UnityEngine;
using System.Collections;
using UnityEngine.UI;


namespace SuperScrollView
{

    [System.Serializable]
    public class PageData
    {
        public GameObject mGameObject;
        public LoopListView mLoopListView;
        public string mTitle;
        public int mItemCount;
    }

    public class PageManager : MonoBehaviour
    {
        [System.Serializable]
        public class PageFadeTypeData
        {
            public Image.FillMethod fillMethod;
            public int fillOrign;
        }

        static PageManager instance = null;

        public PageData[] mAllPageList;

        public float mPageFadeTotalTime = 2;

        int mCurShowingPageIndex;


        public GameObject mDemoPageRootObj;
        public GameObject mPageTransferRootObj;

        public PageFadeTypeData[] mPageFadeDataArray;

        Image mPageTransferMaskImage;

        bool mIsChangingPage;

        GameObject mCurFadingPageObj;

        float mPageFadeLeftTime;

        int mTransferCount = 0;

        public static PageManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Object.FindObjectOfType<PageManager>();
                }
                return instance;
            }

        }


        public void ResetData()
        {
            mPageTransferMaskImage = mPageTransferRootObj.GetComponent<Image>();
            foreach (PageData obj in mAllPageList)
            {
                obj.mGameObject.SetActive(false);
            }
            mIsChangingPage = false;
            mCurShowingPageIndex = 0;
            GetPageObj(0).SetActive(true);
            mCurFadingPageObj = null;
        }



        // Update is called once per frame
        void Update()
        {
            if (mIsChangingPage)
            {
                mPageFadeLeftTime -= Time.deltaTime;
                if (mPageFadeLeftTime <= 0)
                {
                    mIsChangingPage = false;
                    mCurFadingPageObj.SetActive(false);
                    mCurFadingPageObj.transform.SetParent(mDemoPageRootObj.transform);
                }
                else
                {
                    mPageTransferMaskImage.fillAmount = mPageFadeLeftTime / mPageFadeTotalTime;
                }
            }
        }

        public GameObject GetPageObj(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= mAllPageList.Length)
            {
                return null;
            }
            return mAllPageList[pageIndex].mGameObject;
        }

        public PageData GetPageData(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= mAllPageList.Length)
            {
                return null;
            }
            return mAllPageList[pageIndex];
        }

        public PageData GetCurShowingPageData()
        {
            if (mCurShowingPageIndex < 0 || mCurShowingPageIndex >= mAllPageList.Length)
            {
                return null;
            }
            return mAllPageList[mCurShowingPageIndex];
        }

        public void ShowPanel(int panelIndex)
        {
            if (mIsChangingPage)
            {
                return;
            }

            if (mCurShowingPageIndex == panelIndex)
            {
                return;
            }

            mTransferCount++;

            GameObject curPageObj = GetPageObj(mCurShowingPageIndex);

            GameObject targetPageObj = GetPageObj(panelIndex);

            targetPageObj.SetActive(true);

            curPageObj.transform.SetParent(mPageTransferRootObj.transform);
            mCurFadingPageObj = curPageObj;

            int fadeType = mTransferCount % mPageFadeDataArray.Length;

            mPageTransferMaskImage.fillMethod = mPageFadeDataArray[fadeType].fillMethod;
            mPageTransferMaskImage.fillOrigin = mPageFadeDataArray[fadeType].fillOrign;

            mPageTransferMaskImage.fillAmount = 1;

            mCurShowingPageIndex = panelIndex;

            mPageFadeLeftTime = mPageFadeTotalTime;
            mIsChangingPage = true;
        }

        public bool IsShowingFirstPage()
        {
            return (mCurShowingPageIndex == 0);
        }

        public bool IsShowingLastPage()
        {
            return (mCurShowingPageIndex == (mAllPageList.Length - 1));
        }

        public int CurShowingPageIndex
        {
            get
            {
                return mCurShowingPageIndex;
            }
        }
        public int PageCount
        {
            get
            {
                return mAllPageList.Length;
            }
        }

    }
}
