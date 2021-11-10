//
//  TCGBNotificationOptions.h
//  Gamebase
//
//  Created by NHN on 2020/08/05.
//  Copyright © 2020 NHN Corp. All rights reserved.
//

#ifndef TCGBNotificationOptions_h
#define TCGBNotificationOptions_h

#import <Foundation/Foundation.h>
#import "TCGBValueObject.h"

NS_ASSUME_NONNULL_BEGIN

@interface TCGBNotificationOptions : NSObject <TCGBValueObject>

@property (nonatomic) BOOL foregroundEnabled;
@property (nonatomic) BOOL badgeEnabled;
@property (nonatomic) BOOL soundEnabled;

+ (TCGBNotificationOptions *)notificationOptionsWithForegroundEnabled:(BOOL)foregroundEnabled badgeEnabled:(BOOL)badgeEnabled soundEnabled:(BOOL)soundEnabled;
+ (nullable TCGBNotificationOptions *)notificationOptionsWithJSONString:(NSString *)jsonString;

#pragma mark - Utility
- (NSString *)jsonString;
- (NSString *)prettyJsonString;
@end

NS_ASSUME_NONNULL_END

#endif /* TCGBNotificationOptions_h */
