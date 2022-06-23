using System.Collections.Generic;
using UnityEngine;

using Doozy.Runtime.Signals;

namespace PIERStory
{
    public class PopupLackOfCurrency : PopupBase
    {
        [Space(15)]
        public Sprite spriteStar;
        public Sprite spriteCoin;
        public UnityEngine.UI.Image lackOfCurrencyImage;

        [Space]
        public List<BaseStarProduct> starProducts;
        public List<BaseCoinExchangeProduct> coinExchangeProducts;

        [Space]
        public GeneralPackProduct packageProduct;

        public override void Show()
        {
            if(isShow)            
                return;
                
            base.Show();

            // isPositive : true면 스타부족, false면 코인부족
            lackOfCurrencyImage.sprite = Data.isPositive ? spriteStar : spriteCoin;

            CurrencySetting();
            PackageSetting();
        }


        /// <summary>
        /// 재화 상품 세팅
        /// </summary>
        void CurrencySetting()
        {
            // 코인이 부족한 경우의 코인 환전 세팅
            if (!Data.isPositive)
            {
                foreach (BaseCoinExchangeProduct ce in coinExchangeProducts)
                    ce.InitExchangeProduct();

                // 필요한 값보다 많아질 때까지 재설정
                while (coinExchangeProducts[0].quantity < Data.contentValue)
                {
                    foreach (BaseCoinExchangeProduct ce in coinExchangeProducts)
                    {
                        int productId = int.Parse(ce.exchangeProductID) + 1;
                        ce.exchangeProductID = productId.ToString();
                        ce.InitExchangeProduct();
                    }
                }

                // 만약 코인 환전할 최소한의 스타마저 없다면?
                if (!UserManager.main.CheckGemProperty(coinExchangeProducts[0].price))
                {
                    starProducts[0].InitProduct(FindStarProductCloseQuantity());
                    starProducts[0].gameObject.SetActive(true);

                    coinExchangeProducts[1].exchangeProductID = coinExchangeProducts[0].exchangeProductID;
                    coinExchangeProducts[1].InitExchangeProduct();
                    coinExchangeProducts[1].gameObject.SetActive(true);
                    return;
                }

                foreach (BaseCoinExchangeProduct ce in coinExchangeProducts)
                    ce.gameObject.SetActive(true);
            }
            else
            {
                // 스타상품 세팅
                List<string> productIdList = ProductionList("ifyou_star_");

                int n = 0;
                string randomId = string.Empty;

                if (productIdList.Count < 1)
                {
                    Debug.Log("<color=purple>스타 상품 없음!</color>");
                    return;
                }

                while (n < 2)
                {
                    randomId = productIdList[Random.Range(0, productIdList.Count)];

                    // 첫번째꺼랑 상품 똑같으면 다시 돌리기
                    if (n > 0 && starProducts[0].productID == randomId)
                        continue;

                    starProducts[n].InitProduct(randomId);
                    starProducts[n].gameObject.SetActive(true);
                    n++;
                }
            }
        }

        /// <summary>
        /// 패키지 세팅
        /// </summary>
        void PackageSetting()
        {
            List<string> productIdList = ProductionList("_pack");

            if (productIdList.Count < 1)
            {
                Debug.Log("<color=purple>패키지 상품 없음!</color>");
                return;
            }

            int randomIndexValue = Random.Range(0, productIdList.Count);
            packageProduct.InitPackage(productIdList[randomIndexValue], BillingManager.main.GetGameProductItemMasterInfo(productIdList[randomIndexValue]));
        }


        public void OnClickGoShopButton()
        {
            // 상점 열어주고
            Signal.Send(LobbyConst.STREAM_COMMON, "Shop", string.Empty);

            // 닫기~
            Hide();
        }


        /// <summary>
        /// 요구치에 가장 근접한 스타상품 찾기
        /// </summary>
        /// <returns></returns>
        string FindStarProductCloseQuantity()
        {
            List<string> productIdList = new List<string>();

            // 스타상품 모두 찾기
            for (int i = 0; i < BillingManager.main.productMasterJSON.Count; i++)
            {
                // 스타 상품 아니면 continue
                if (!SystemManager.GetJsonNodeString(BillingManager.main.productMasterJSON[i], "product_id").Contains("ifyou_star_"))
                    continue;

                if (SystemManager.GetJsonNodeInt(BillingManager.main.productMasterJSON[i], "max_count") == -1)
                {
                    productIdList.Add(SystemManager.GetJsonNodeString(BillingManager.main.productMasterJSON[i], "product_id"));
                    continue;
                }

                // max_count가 존재(상시 구매X)하는데 이미 max_count만큼 샀으면 넘어가기
                if (BillingManager.main.CheckProductPurchaseCount(SystemManager.GetJsonNodeString(BillingManager.main.productMasterJSON[i], "product_master_id")) >=
                    SystemManager.GetJsonNodeInt(BillingManager.main.productMasterJSON[i], "max_count"))
                    continue;

                productIdList.Add(SystemManager.GetJsonNodeString(BillingManager.main.productMasterJSON[i], "product_id"));
            }

            List<BaseStarProduct> allStarProductList = new List<BaseStarProduct>();

            // 모든 스타 상품들 리스트에 넣기
            foreach (string id in productIdList)
            {
                BaseStarProduct sp = new BaseStarProduct();
                sp.InitProductData(id);
                allStarProductList.Add(sp);
            }

            int minValue = int.MaxValue, totalQuantity = 0, calcMin = 0;
            string closeProductId = string.Empty;

            foreach (BaseStarProduct sp in allStarProductList)
            {
                totalQuantity = sp.mainGemQuantity + sp.subGemQuantity + sp.firstPurchaseBonusGem;

                // 총합이 필요한 값보다 적으면 이 상품은 아닌거야
                if (totalQuantity < coinExchangeProducts[0].price)
                    continue;

                calcMin = totalQuantity - coinExchangeProducts[0].price;

                // 제일 최소값을 계속 갱신해줘서 해당 상품ID를 저장해둔다
                if (calcMin < minValue)
                {
                    minValue = calcMin;
                    closeProductId = sp.productID;
                }
            }

            return closeProductId;
        }


        /// <summary>
        /// 상품 리스트 리턴해주기
        /// </summary>
        /// <param name="productionType">상품 타입</param>
        List<string> ProductionList(string productionType)
        {
            List<string> productIdList = new List<string>();

            for (int i = 0; i < BillingManager.main.productMasterJSON.Count; i++)
            {
                // 상품이 상점만 공개면 넘어가고, 해당 상품이 아니어도 넘어간다
                if (SystemManager.GetJsonNodeInt(BillingManager.main.productMasterJSON[i], "is_public") == 1 ||
                    !SystemManager.GetJsonNodeString(BillingManager.main.productMasterJSON[i], "product_id").Contains(productionType))
                    continue;

                // 팝업, 모두 공개 중 상시 판매 상품도 리스트에 추가하고 다음 상품 체크
                if (SystemManager.GetJsonNodeInt(BillingManager.main.productMasterJSON[i], "max_count") == -1)
                {
                    productIdList.Add(SystemManager.GetJsonNodeString(BillingManager.main.productMasterJSON[i], "product_id"));
                    continue;
                }

                // max_count가 존재(상시 구매X)하는데 이미 max_count만큼 샀으면 넘어가기
                if (BillingManager.main.CheckProductPurchaseCount(SystemManager.GetJsonNodeString(BillingManager.main.productMasterJSON[i], "product_master_id")) >=
                    SystemManager.GetJsonNodeInt(BillingManager.main.productMasterJSON[i], "max_count"))
                    continue;

                productIdList.Add(SystemManager.GetJsonNodeString(BillingManager.main.productMasterJSON[i], "product_id"));
            }

            return productIdList;
        }
    }
}