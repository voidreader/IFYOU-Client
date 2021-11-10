//
//  TCGBImageNoticeConfiguration.h
//  Gamebase
//
//  Created by NHN on 2020/06/16.
//  Copyright © 2020 NHN Corp. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

#ifndef TCGBImageNoticeConfiguration_h
#define TCGBImageNoticeConfiguration_h

NS_ASSUME_NONNULL_BEGIN

@interface TCGBImageNoticeConfiguration : NSObject

/**---------------------------------------------------------------------------------------
 * @name background
 *  ---------------------------------------------------------------------------------------
 */

/**
 Image Notices background color
 
 @warning Default value is (R=0, G=0, B=0, a=0.5)
 */
@property (nonatomic, strong) UIColor* backgroundColor;

/**---------------------------------------------------------------------------------------
* @name timeout
*  ---------------------------------------------------------------------------------------
*/

/**
Server Timeout.

The unit is millseconds.
@warning Default value is 5000(ms).
*/
@property (nonatomic, assign) long timeoutMS;

/**---------------------------------------------------------------------------------------
* @name auto close
*  ---------------------------------------------------------------------------------------
*/

/**
Boolean value.

When set to YES, close image notices when the custom event comes.
@warning Default value is YES.
*/
@property (nonatomic, assign) BOOL enableAutoCloseByCustomScheme;
@end

NS_ASSUME_NONNULL_END

#endif /* TCGBImageNoticeConfiguration_h */
