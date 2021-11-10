//
//  TCGBTransferAccountRenewConfiguration.h
//  Gamebase
//
//  Created by NHN on 07/02/2019.
//  © NHN Corp. All rights reserved.
//
#import <Foundation/Foundation.h>
#import "TCGBTransferAccountConstants.h"

#ifndef TCGBTransferAccountRenewConfiguration_h
#define TCGBTransferAccountRenewConfiguration_h

NS_ASSUME_NONNULL_BEGIN

@interface TCGBTransferAccountRenewConfiguration : NSObject

@property (nonatomic, assign) TCGBTransferAccountRenewalModeType renewalMode;
@property (nonatomic, assign) TCGBTransferAccountRenewalTargetType renewalTarget;
@property (nonatomic, strong, nullable) NSString* accountId;
@property (nonatomic, strong, nullable) NSString* accountPassword;

- (instancetype)init __attribute__((unavailable("init not available")));
- (instancetype)initWithRenewalMode:(TCGBTransferAccountRenewalModeType)renewalMode
                      renewalTarget:(TCGBTransferAccountRenewalTargetType)renewalTarget
                          accountId:(nullable NSString *)accountId
                    accountPassword:(nullable NSString *)accountPassword;

+ (TCGBTransferAccountRenewConfiguration *)autoRenewConfigurationWithRenewalTarget:(TCGBTransferAccountRenewalTargetType)renewalTarget;
+ (TCGBTransferAccountRenewConfiguration *)manualRenewConfigurationWithAccountPassword:(NSString *)accountPassword;
+ (TCGBTransferAccountRenewConfiguration *)manualRenewConfigurationWithAccountId:(NSString *)accountId
                                                              accountPassword:(NSString *)accountPassword;

- (NSString *)JSONString;
- (NSString *)JSONPrettyString;

@end

NS_ASSUME_NONNULL_END

#endif /* TCGBTransferAccountRenewConfiguration_h */
