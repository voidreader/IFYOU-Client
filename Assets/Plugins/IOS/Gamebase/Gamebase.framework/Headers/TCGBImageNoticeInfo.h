//
//  TCGBImageNoticeInfo.h
//  Gamebase
//
//  Created by NHN on 2020/06/11.
//  Copyright © 2020 NHN Corp. All rights reserved.
//
#import "TCGBValueObject.h"
#import <Foundation/Foundation.h>

#ifndef TCGBImageNoticeInfo_h
#define TCGBImageNoticeInfo_h

NS_ASSUME_NONNULL_BEGIN

@interface TCGBImageNoticeInfo : NSObject <TCGBValueObject>

/**
 Unique  identifier for each notice.
 This parameter is used for the purpose of distinguishing each notice and preventing duplicate display.
 */
@property (nonatomic, assign) long imageNoticeId;

/**
 Create webView url by adding domain and this value.
 */
@property (nonatomic, strong) NSString *path;

/**
 This value has a url that should be moved when the popup is clicked,
 or a custom scheme that should fire and event.
 */
@property (nonatomic, strong, nullable) NSString *clickScheme;

/**
 This value represents the action when the popup is clicked.
 It has one of the two values below.
 "openUrl", "custom"
 */
@property (nonatomic, strong) NSString *clickType;

/**
 This value is only used on 'WINDOWS' platform.
 this value is used to determine the popup theme.
 It has one of the two values below.
 "DARK", "LIGHT".
 */
@property (nonatomic, strong) NSString *theme;

/**
This value represents the width of the image.
*/
@property (nonatomic, assign) int width;

/**
This value represents the height of the image.
*/
@property (nonatomic, assign) int height;

/**
 This value represents the footer height of the imageNotice.
 */
@property (nonatomic, assign) int footerHeight;

+ (nullable NSArray *)imageNoticeInfoListWithResult:(nullable NSDictionary *)result;

@end

NS_ASSUME_NONNULL_END

#endif /* TCGBImageNoticeInfo_h */
