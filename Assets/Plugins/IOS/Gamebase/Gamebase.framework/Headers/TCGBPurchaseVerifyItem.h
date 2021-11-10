//
//  TCGBPurchaseVerifyItem.h
//  Gamebase
//
//  Created by NHN on 2020/10/30.
//  Copyright © 2020 NHN Corp. All rights reserved.
//

#ifndef TCGBPurchaseVerifyItem_h
#define TCGBPurchaseVerifyItem_h

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface TCGBPurchaseVerifyItem : NSObject

@property (nonatomic, assign) BOOL isVerified;
@property (nonatomic, assign) BOOL hasInvalidItem;
@property (nonatomic, strong, nullable) NSMutableArray *toastGamebaseProducts;
@property (nonatomic, strong, nullable) NSMutableArray *gamebasePurchasableItems;

+ (TCGBPurchaseVerifyItem *)sharedPurchaseVerifyItem;
+ (TCGBPurchaseVerifyItem *)initializeInstance;

- (void) addToastGamebaseProduct:(NSDictionary *)toastGamebaseProduct;
- (void) addGamebasePurchasableItem:(NSDictionary *)gamebasePurchasableItem;

- (NSString *)JSONString;
- (NSString *)JSONPrettyString;

@end

NS_ASSUME_NONNULL_END

#endif /* TCGBPurchaseVerifyItem_h */
