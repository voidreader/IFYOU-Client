#import "TCGBPurchasePlugin.h"
#import "DelegateManager.h"
#import <Gamebase/Gamebase.h>
#import "TCGBJsonUtil.h"
#import "NativeMessage.h"
#import "TCGBPluginData.h"
#import "EngineMessage.h"
#import "TCGBViewControllerManager.h"

#define PURCHASE_API_REQUEST_PURCHASE_SEQ                   	@"gamebase://requestPurchaseSeq"
#define PURCHASE_API_REQUEST_PURCHASE_PRODUCTID                 @"gamebase://requestPurchaseProductId"
#define PURCHASE_API_REQUEST_PURCHASE_PRODUCTID_WITH_PAYLOAD    @"gamebase://requestPurchaseProductIdWithPayload"
#define PURCHASE_API_REQUEST_ITEM_LIST_OF_NOT_CONSUMED      	@"gamebase://requestItemListOfNotConsumed"
#define PURCHASE_API_REQUEST_RETYR_TRANSACTION              	@"gamebase://requestRetryTransaction"
#define PURCHASE_API_REQUEST_ITEM_LIST_PURCHASABLE          	@"gamebase://requestItemListPurchasable"
#define PURCHASE_API_REQUEST_ITEM_LIST_AT_AP_CONSOLE        	@"gamebase://requestItemListAtIAPConsole"
#define PURCHASE_API_SET_PROMOTION_IAP_HANDLER              	@"gamebase://setPromotionIAPHandler"
#define PURCHASE_API_SET_STORE_CODE                         	@"gamebase://setStoreCode"
#define PURCHASE_API_GET_STORE_CODE                         	@"gamebase://getStoreCode"
#define PURCHASE_API_REQUEST_ACTIVATED_PURCHASES            	@"gamebase://requestActivatedPurchases"

@implementation TCGBPurchasePlugin

- (instancetype)init {
    if ((self = [super init]) == nil) {
        return nil;
    }
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:PURCHASE_API_REQUEST_PURCHASE_SEQ target:self selector:@selector(requestPurchaseSeq:)];
	
	[[DelegateManager sharedDelegateManager] addAsyncDelegate:PURCHASE_API_REQUEST_PURCHASE_PRODUCTID target:self selector:@selector(requestPurchaseProductId:)];
	
	[[DelegateManager sharedDelegateManager] addAsyncDelegate:PURCHASE_API_REQUEST_PURCHASE_PRODUCTID_WITH_PAYLOAD target:self selector:@selector(requestPurchaseProductIdWithPayload:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:PURCHASE_API_REQUEST_ITEM_LIST_OF_NOT_CONSUMED target:self selector:@selector(requestItemListOfNotConsumed:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:PURCHASE_API_REQUEST_RETYR_TRANSACTION target:self selector:@selector(requestRetryTransaction:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:PURCHASE_API_REQUEST_ITEM_LIST_PURCHASABLE target:self selector:@selector(requestItemListPurchasable:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:PURCHASE_API_REQUEST_ITEM_LIST_AT_AP_CONSOLE target:self selector:@selector(requestItemListAtIAPConsole:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:PURCHASE_API_SET_PROMOTION_IAP_HANDLER target:self selector:@selector(setPromotionIAPHandler:)];
    
    [[DelegateManager sharedDelegateManager] addSyncDelegate:PURCHASE_API_SET_STORE_CODE target:self selector:@selector(setStoreCode:)];
    
    [[DelegateManager sharedDelegateManager] addSyncDelegate:PURCHASE_API_GET_STORE_CODE target:self selector:@selector(getStoreCode:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:PURCHASE_API_REQUEST_ACTIVATED_PURCHASES target:self selector:@selector(requestActivatedPurchases:)];
    return self;
}

-(void)requestPurchaseSeq:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    NSDictionary* convertedDic = [message.jsonData JSONDictionary];
    [TCGBPurchase requestPurchaseWithItemSeq:[convertedDic[@"itemSeq"] longValue] viewController:[[TCGBViewControllerManager sharedGamebaseViewControllerManager] getViewController] completion:^(TCGBPurchasableReceipt *purchasableReceipt, TCGBError *error) {     
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:[purchasableReceipt JSONString] extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);
    }];
}

-(void)requestPurchaseProductId:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    NSDictionary* convertedDic = [message.jsonData JSONDictionary];
    [TCGBPurchase requestPurchaseWithGamebaseProductId:convertedDic[@"gamebaseProductId"] viewController:[[TCGBViewControllerManager sharedGamebaseViewControllerManager] getViewController] completion:^(TCGBPurchasableReceipt *purchasableReceipt, TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:[purchasableReceipt JSONString] extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);
    }];
}

-(void)requestPurchaseProductIdWithPayload:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    NSDictionary* convertedDic = [message.jsonData JSONDictionary];
    [TCGBPurchase requestPurchaseWithGamebaseProductId:convertedDic[@"gamebaseProductId"] payload:convertedDic[@"payload"] viewController:[[TCGBViewControllerManager sharedGamebaseViewControllerManager] getViewController] completion:^(TCGBPurchasableReceipt *purchasableReceipt, TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:[purchasableReceipt JSONString] extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);
    }];
}

-(void)requestItemListOfNotConsumed:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    [TCGBPurchase requestItemListOfNotConsumedWithCompletion:^(NSArray<TCGBPurchasableReceipt *> *purchasableReceiptArray, TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:[purchasableReceiptArray JSONString] extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);
    }];
}

-(void)requestRetryTransaction:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    [TCGBPurchase requestRetryTransactionWithCompletion:^(TCGBPurchasableRetryTransactionResult *transactionResult, TCGBError *error) {
        NSString* jsonString = @"";
        if(transactionResult != nil)
        {
            jsonString = [transactionResult description];
        }
        
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:jsonString extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);
    }];
}

-(void)requestItemListPurchasable:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    [TCGBPurchase requestItemListPurchasableWithCompletion:^(NSArray<TCGBPurchasableItem *> *purchasableItemArray, TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:[purchasableItemArray JSONString] extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);
    }];
}

-(void)requestItemListAtIAPConsole:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    [TCGBPurchase requestItemListAtIAPConsoleWithCompletion:^(NSArray<TCGBPurchasableItem *> *purchasableItemArray, TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:[purchasableItemArray JSONString] extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);
    }];
}

-(void)setPromotionIAPHandler:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    [TCGBPurchase setPromotionIAPHandler:^(TCGBPurchasableReceipt *purchasableReceipt, TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:[purchasableReceipt JSONString] extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);
    }];
}

-(NSString*)setStoreCode:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    [TCGBPurchase setStoreCode:message.jsonData];
    return @"";
}

-(NSString*)getStoreCode:(TCGBPluginData*)pluginData {
    NSString *storeCode = [TCGBPurchase storeCode];
    
    return storeCode;
}

-(void)requestActivatedPurchases:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    [TCGBPurchase requestActivatedPurchasesWithCompletion:^(NSArray<TCGBPurchasableReceipt *> *purchasableReceiptArray, TCGBError *error) {
        
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:[purchasableReceiptArray JSONString] extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);
    }];
}
@end

