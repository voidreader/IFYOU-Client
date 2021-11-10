//
//  TCGBPurchasable.h
//  Gamebase
//
//  Created by NHN on 2016. 12. 7..
//  © NHN Corp. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "TCGBError.h"

NS_ASSUME_NONNULL_BEGIN

@class TCGBPurchasableItem;
@class TCGBPurchasableReceipt;
@class TCGBPurchasableRetryTransactionResult;

/** The protocol TCGBPurchasePromotionIAPDelegate is for developing IAP Adapter.<br/>
 This protocol contains methods to receive result of promotion purchase.
 */
@protocol TCGBPurchasePromotionIAPDelegate <NSObject>

- (void)didReceivePurchasableReceipt:(nullable TCGBPurchasableReceipt *)purchasableReceipt error:(nullable TCGBError *)error;

@end

/** The protocol TCGBPurchasable is for developing IAP Adapter.<br/>
 This protocol contains several methods such as to request item list, to puchasing item and retrying incompleted purchasing.
 */
@protocol TCGBPurchasable


/**---------------------------------------------------------------------------------------
 * @name Initilaize IAP Adapter
 *  ---------------------------------------------------------------------------------------
 */

/** This method ininialize the IAP Adapter class.
 
 @param appID       appID is ToastCloud IAP's appID, not ToastCloud projectID.
 @param zoneType       zoneType is ToastCloud IAP's zoneType.
 @param store       store should be appstore. Others will be ignored.
 @param userID      userID is obtained after logged in. This must be unique cause of tracing purchasing.
 @param isDebugMode isDebugMode is for setting debug mode of ToastCloud IAP module.
 */
@optional
- (void)initializePurchaseWithAppID:(NSString *)appID zoneType:(NSString *)zoneType store:(NSString *)store userID:(NSString *)userID enableDebugMode:(BOOL)isDebugMode;


@required
/**---------------------------------------------------------------------------------------
 * @name Request Item List
 *  ---------------------------------------------------------------------------------------
 */

/** This is the primary method for obtaining ItemList which is registered at ToastCloud IAP Console and Apple Itunes Connect.
 
 Request a item list which is purchasable. This list has items which are registered in both Market(AppStore) and ToastCloud IAP Console.
 
 @param completion      completion may return the NSArray of TCGBPurchasableItem.<br/>
 If there is an error, TCGBError will be returned.
 @warning   You should call this method after ```logged in```, otherwise you will get **TCGB_ERROR_NOT_LOGGED_IN** error in the completion.
 */
- (void)requestItemListPurchasableWithCompletion:(void(^)(NSArray<TCGBPurchasableItem *> * _Nullable purchasableItemArray, TCGBError * _Nullable error))completion;


/** This is the method for obtaining ItemList which is registered at ToastCloud IAP Console.
 
 Request a item list which is purchasable. This list has items which are only registered in ToastCloud IAP Console, not Market(AppStore)
 
 @param completion      completion may return the NSArray of TCGBPurchasableItem.<br/>
 If there is an error, TCGBError will be returned.
 @warning   You should call this method after ```logged in```, otherwise you will get **TCGB_ERROR_NOT_LOGGED_IN** error in the completion.
 */
- (void)requestItemListAtIAPConsoleWithCompletion:(void(^)(NSArray<TCGBPurchasableItem *> * _Nullable purchasableItemArray, TCGBError * _Nullable error))completion;


/**---------------------------------------------------------------------------------------
 * @name Request Purchasing Item
 *  ---------------------------------------------------------------------------------------
 */

/** This is the method to request purchasing item which identifier is itemSeq. There is a viewController parameter and you may put your top most viewController. If you don't, this method will find out top most view controller and put it in the parameter.
 
 Request Purchasing Item that has itemId.
 
 @param itemSeq         itemID which you want to purchase.
 @param viewController  represent to current viewcontroller.
 @param completion      completion may return the TCGBPurchasableReceipt instance.<br/>
 If there is an error, TCGBError will be returned.
 @warning   You should call this method after ```logged in```, otherwise you will get **TCGB_ERROR_NOT_LOGGED_IN** error in the completion.
 */
- (void)requestPurchaseWithItemSeq:(long)itemSeq gamebasePayload:(nullable NSString *)gamebasePayload viewController:(UIViewController *)viewController completion:(void(^)(TCGBPurchasableReceipt * _Nullable purchasableReceipt, TCGBError * _Nullable error))completion;

- (void)requestPurchaseWithMarketItemId:(NSString *)marketItemId viewController:(UIViewController *)viewController completion:(void(^)(TCGBPurchasableReceipt * _Nullable purchasableReceipt, TCGBError * _Nullable error))completion;

- (void)requestPurchaseWithMarketItemId:(NSString *)productId payload:(nullable NSString *)payload gamebasePayload:(nullable NSString *)gamebasePayload viewController:(UIViewController *)viewController completion:(void(^)(TCGBPurchasableReceipt * _Nullable purchasableReceipt, TCGBError * _Nullable error))completion;

/**---------------------------------------------------------------------------------------
 * @name Request non-consumed Item List
 *  ---------------------------------------------------------------------------------------
 */

/** This method provides an item list that have non-consumed.
 
 Request a Item List which is not consumed. You should deliver this itemReceipt to your game server to consume it or request consuming API to ToastCloud IAP Server. You may call this method after logged in and deal with these non-consumed items.
 
 @param completion      completion may return the NSArray of TCGBPurchasableReceipt instances.<br/>
 This instance has the paymentSequence, itemSequence, PurchaseToken information.<br/>
 If there is an error, TCGBError will be returned.
 @warning   You should call this method after ```logged in```, otherwise you will get **TCGB_ERROR_NOT_LOGGED_IN** error in the completion.
 */
- (void)requestItemListOfNotConsumedWithCompletion:(void(^)(NSArray<TCGBPurchasableReceipt *> * _Nullable purchasableReceiptArray, TCGBError * _Nullable error))completion;


/**---------------------------------------------------------------------------------------
 * @name Request retry purchasing processes
 *  ---------------------------------------------------------------------------------------
 */

/** This method is for retrying failed purchasing proccesses.
 
 Request a retrying transaction which is not completed to IAP Server
 
 @param completion      completion may return the TCGBPurchasableRetryTransactionResult which has two member variables which are named 'successList' and 'failList'.<br/>
 These two variables are array of TCGBPurchasableReceipt.<br/>
 Each key has a list of TCGBPurchasableReceipt that has uncompleted purchase information.<br/>
 If there is an error, TCGBError will be returned.
 */
- (void)requestRetryTransactionWithCompletion:(void(^)(TCGBPurchasableRetryTransactionResult * _Nullable transactionResult, TCGBError * _Nullable error))completion;

- (void)requestActivatedPurchasesWithCompletion:(void(^)(NSArray<TCGBPurchasableReceipt *> * _Nullable purchasableReceiptArray, TCGBError * _Nullable error))completion;

- (void)requestRestoreWithCompletion:(void(^)(NSArray<TCGBPurchasableReceipt *> * _Nullable purchasableReceiptArray, TCGBError * _Nullable error))completion;

@property (nonatomic, weak, nullable) id<TCGBPurchasePromotionIAPDelegate> promotionDelegate;

@end


#pragma mark - TCGBPurchasableItem
/** The TCGBPurchasableItem class is VO class of item entity.
 */
@interface TCGBPurchasableItem : NSObject

/**---------------------------------------------------------------------------------------
 * @name Properties
 *  ---------------------------------------------------------------------------------------
 */

/** itemSeq
 
 Item Sequence which is the number presented in Toast Cloud IAP Console.
*/
@property (assign)            long itemSeq;

/** item price
 
 This value is from the market.
 @warning If there is no price data, it will be initialized to -1
 */
@property (assign)            float price;

/** item name
 
 Item name is from Toast Cloud IAP Console.
 */
@property (nonatomic, strong) NSString *itemName;

/** marketId
 
 which is actually **AS**.
 */
@property (nonatomic, strong) NSString *marketId;

/** marketItemId
 
 ItemID which is registered at market(itunesconnect).
 */
@property (nonatomic, strong) NSString *marketItemId;

/** gamebaseProductId

ItemID which is registered at IAP Console.
*/
@property (nonatomic, strong) NSString *gamebaseProductId;

/** currency
 currency which is registered at market.
*/
@property (nonatomic, strong) NSString *currency;

/** usingStatus
 
 This string value represent if this item is available.
 */
@property (nonatomic, strong) NSString *usingStatus;

/** localizedPrice
 
 This string value represent price with currency.
 */
@property (nonatomic, strong) NSString *localizedPrice;

/** localizedTitle
 localizedTitle which is registered at market.
*/
@property (nonatomic, strong) NSString *localizedTitle;

/** localizedDescription
 localizedDescription which is registered at market.
*/
@property (nonatomic, strong) NSString *localizedDescription;

/** productType
 
 This string value represent type of product. (CONSUMABLE, UNKNOWN, AUTO_RENEWABLE, CONSUMABLE_AUTO_RENEWABLE)
 */

@property (nonatomic, strong) NSString *productType;

/** active
 
 This boolean value represent
 */
@property (nonatomic, assign, getter=isActive) BOOL active;

/**---------------------------------------------------------------------------------------
 * @name Allocation
 *  ---------------------------------------------------------------------------------------
 */

/** Initialize the class with JSON Value.
 
 @param result  result is a json formatted NSDictionary object. This is from Gamebase Server.
 @return    Instance being initialized.
 */
+ (instancetype)gamebaseProductWithDictionary:(NSDictionary *)result;

/** Initialize the class with JSON Value.
 
 @param result      result is a json formatted NSDictionary object. This is from ToastCloud IAP Server.
 @return Instance being initialized.
 */
+ (instancetype)toastProductWithDictionary:(NSDictionary*)result;

- (instancetype)initWithDictionary:(NSDictionary *)result;

- (instancetype)initWithItemSeq:(long)itemSeq
                          price:(float)price
                       itemName:(NSString *)itemName
                    usingStatus:(NSString *)usingStatus
                       marketId:(NSString *)marketId
                   marketItemId:(NSString *)marketItemId
              gamebaseProductId:(NSString *)gamebaseProductId
                       currency:(NSString *)currency
                 localizedPrice:(NSString *)localizedPrice
                 localizedTitle:(NSString *)localizedTitle
           localizedDescription:(NSString *)localizedDescription
                    productType:(NSString *)productType
                         active:(BOOL)active;

- (NSDictionary *)dictionaryPurchasableItem;
- (NSString *)JSONString;
- (NSString *)JSONPrettyString;

@end




#pragma mark - TCGBPurchasableReceipt
/** The TCGBPurchasableReceipt class represent a receipt that is from the IAP Server.
 */
@interface TCGBPurchasableReceipt : NSObject

/**---------------------------------------------------------------------------------------
 * @name Properties
 *  ---------------------------------------------------------------------------------------
 */

/** itemSeq
 
 Item Sequence which is the number presented in Toast Cloud IAP Console.
 */
@property (assign)            long itemSeq;

/** marketItemId
 
 ItemID which is registered at market(itunesconnect).
 */
@property (nonatomic, strong) NSString *marketItemId;

/** gamebaseProductId

productID which is registered at IAP Console
*/
@property (nonatomic, strong) NSString *gamebaseProductId;

/** item price
 
 This value is from the market.
 @warning If there is no price data, it will be initialized to -1
 */
@property (assign)            float price;

/** currency
 */
@property (nonatomic, strong) NSString *currency;

/** paymentSeq
 
 Payment Sequence is used to trace purchase transaction.
 */
@property (nonatomic, strong) NSString *paymentSeq;

/** purchaseToken
 
 Purchase Token is an unique string to validate purchasement.
 */
@property (nonatomic, strong) NSString *purchaseToken;

/** productType

This string value represent type of product. (CONSUMABLE, UNKNOWN, AUTO_RENEWABLE, CONSUMABLE_AUTO_RENEWABLE)
*/
@property (nonatomic, strong) NSString *productType;

/** userId
 */
@property (nonatomic, strong) NSString *userId;

/** paymentId
 */
@property (nonatomic, strong, nullable) NSString *paymentId;

/** originalPaymentId
 */
@property (nonatomic, strong, nullable) NSString *originalPaymentId;

/** purchaseTime
 */
@property (nonatomic, assign) long purchaseTime;

/**expiryTime
 */
@property (nonatomic, assign) long expiryTime;

/**userPayload
 */
@property (nonatomic, strong, nullable) NSString *payload;

/**---------------------------------------------------------------------------------------
 * @name Allocation
 *  ---------------------------------------------------------------------------------------
 */

/** Initialize the class with JSON Value.
 
 @param result      result is a json formatted NSDictionary object. This is from ToastCloud IAP Server.
 @return Instance being initialized.
 */
+ (instancetype)purchasableReceiptWithResult:(NSDictionary*)result;
+ (nullable instancetype)purchasableReceiptFromJsonString:(NSString *)jsonString;

- (NSString *)JSONString;
- (NSString *)JSONPrettyString;

@end


#pragma mark - TCGBPurchasableRetryTransactionResult
/** The TCGBPurchasableRetryTransactionResult class represent a result after retrying failed purchasing processes.
 */
@interface TCGBPurchasableRetryTransactionResult : NSObject

/**---------------------------------------------------------------------------------------
 * @name Properties
 *  ---------------------------------------------------------------------------------------
 */

/** successList
 
 This array contains results of successed receipt. Each receipt is implemented by `TCGBPurchasableReceipt`.
 */
@property (nonatomic, strong) NSArray<TCGBPurchasableReceipt *> *successList;

/** failList
 
 This array contains results of failed receipt. Each receipt is implemented by `TCGBPurchasableReceipt`.
 */
@property (nonatomic, strong) NSArray<TCGBPurchasableReceipt *> *failList;

/**---------------------------------------------------------------------------------------
 * @name Allocation
 *  ---------------------------------------------------------------------------------------
 */

/** Initialize the class with JSON Value.
 
 @param result      result is a json formatted NSDictionary object. This is from ToastCloud IAP Server.
 @return Instance being initialized.
 */
+ (instancetype)purchasableTransactionResultWithResult:(NSDictionary*)result;

@end

NS_ASSUME_NONNULL_END
