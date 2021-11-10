//
//  TCGBTransferAccountInfo.h
//  Gamebase
//
//  Created by NHN on 07/02/2019.
//  © NHN Corp. All rights reserved.
//
#import <Foundation/Foundation.h>
#import "TCGBTransferAccountConstants.h"

#ifndef TCGBTransferAccountInfo_h
#define TCGBTransferAccountInfo_h

NS_ASSUME_NONNULL_BEGIN

@interface TCGBTransferAccountInfoAccount : NSObject

@property (nonatomic, strong, readonly) NSString * accountId;
@property (nonatomic, strong, readonly, nullable) NSString * accountPassword;

- (instancetype)init __attribute__((unavailable("init not available.")));

- (NSString *)JSONString;
- (NSString *)JSONPrettyString;

@end


@interface TCGBTransferAccountInfoCondition : NSObject

@property (nonatomic, assign, readonly) NSString* transferAccountType;
@property (nonatomic, assign, readonly) NSString* expirationType;
@property (nonatomic, assign, readonly) long long expirationDate;

- (instancetype)init __attribute__((unavailable("init not available.")));

- (NSString *)JSONString;
- (NSString *)JSONPrettyString;

@end


@interface TCGBTransferAccountInfo : NSObject

@property (nonatomic, assign, readonly) NSString* issuedType;
@property (nonatomic, strong, readonly) TCGBTransferAccountInfoAccount* account;
@property (nonatomic, strong, readonly) TCGBTransferAccountInfoCondition* condition;

- (instancetype)init __attribute__((unavailable("init not available.")));

- (NSString *)JSONString;
- (NSString *)JSONPrettyString;

@end

NS_ASSUME_NONNULL_END

#endif /* TCGBTransferAccountInfo_h */
