//
//  TCGBPushDelegate.h
//  Gamebase
//
//  Created by NHNEnt on 28/08/2019.
//  Copyright © 2019 NHN Corp. All rights reserved.
//

#ifndef TCGBPushDelegate_h
#define TCGBPushDelegate_h

#import "TCGBPush.h"
#import "TCGBError.h"

NS_ASSUME_NONNULL_BEGIN

/** The protocol TCGBPushDelegate is for developing Push Adapter.
 */
@protocol TCGBPushDelegate <NSObject>

- (void)registerPushWithPushConfiguration:(TCGBPushConfiguration *)configuration completion:(void(^)(TCGBError * _Nullable error))completion;

- (void)queryPushWithCompletion:(void(^)(TCGBPushConfiguration * _Nullable configuration, TCGBError * _Nullable error))completion;

- (void)queryTokenInfoWithCompletion:(void(^)(TCGBPushTokenInfo * _Nullable tokenInfo, TCGBError * _Nullable error))completion;

- (void)setSandboxMode:(BOOL)isSandbox;

- (nullable TCGBNotificationOptions *)notificationOptions;

- (void)setAppKey:(NSString *)appKey serviceZone:(NSString *)serviceZone configuration:(TCGBPushConfiguration *)configuration notificationOptions:(nullable TCGBNotificationOptions *)notificationOptions;

@end

NS_ASSUME_NONNULL_END

#endif /* TCGBPushDelegate_h */
