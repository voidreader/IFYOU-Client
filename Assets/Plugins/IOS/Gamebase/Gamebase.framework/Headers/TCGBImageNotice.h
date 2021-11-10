//
//  TCGBImageNotice.h
//  Gamebase
//
//  Created by NHN on 2020/06/15.
//  Copyright © 2020 NHN Corp. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <WebKit/WebKit.h>

@class TCGBError;
@class TCGBImageNoticeConfiguration;

#ifndef TCGBImageNotice_h
#define TCGBImageNotice_h

NS_ASSUME_NONNULL_BEGIN

@interface TCGBImageNotice : NSObject

/**---------------------------------------------------------------------------------------
 * @name Request Showing Image Notices
 *  ---------------------------------------------------------------------------------------
 */

/** This is the method to request showing image notices. There is a viewController parameter and you may put your top most viewController. When finished showing image notices, this method calls closeCompletion.
 
 Request Showing Image Notices.
 
 @param viewController represent to current viewController
 @warning If viewController is nil, Image Notices set it to top most view controller automatically.
 @param closeCompletion completion may call when finished showing image notices.
 If there is an error, TCGBError will be returned.
 
 ### Usage Example
 
    - (void)showImageNotices {
        [TCGBImageNotice showImageNoticesWithViewController:self closeCompletion:^(TCGBError *error) {
            if (error) {
                dispatch_async(dispatch_get_main_queue(), ^{
                    NSLog(@"Error : %@", error);
                });
            } else {
                dispatch_async(dispatch_get_main_queue(), ^{
                    NSLog(@"Finish Image Notices");
                });
            }
        }];
    }
 
 */
 + (void)showImageNoticesWithViewController:(nullable UIViewController *)viewController
                            closeCompletion:(void(^ _Nullable)(TCGBError * _Nullable error))closeCompletion;

/** This is the method to request showing image notices. There is a viewController parameter and you may put your top most viewController. When finished showing image notices, this method calls closeCompletion.

Request Showing Image Notices.

@param viewController Represent to current viewController.
@warning If viewController is nil, Image Notices set it to top most view controller automatically.
@param configuration This configuration is applied to the behavior of image notices.
@warning If configuration is nil, Image Notices set it to default value. it is described in `TCGBImageNoticeConfiguration`.
@param closeCompletion Completion may call when finished showing image notices.
@param schemeEvent SchemeEvent may call when custom scheme event occurred.
If there is an error, TCGBError will be returned.
 
### Usage Example

   - (void)showImageNotices {
       TCGBImageNoticeConfiguration *configuration = [[TCGBImageNoticeConfiguration alloc] init];
       configuration.backgroundColor = [UIColor colorWithRed:0 green:0 blue:0 alpha:0.5];
       configuration.timeoutMS = 5000;
       configuration.enableAutoCloseByCustomScheme = YES;
       [TCGBImageNotice showImageNoticesWithViewController:self configuration:configuration closeCompletion:^(TCGBError *error) {
           if (error) {
               dispatch_async(dispatch_get_main_queue(), ^{
                   NSLog(@"Error : %@", error);
               });
           } else {
               dispatch_async(dispatch_get_main_queue(), ^{
                   NSLog(@"Finish Image Notices");
               });
           }
       } schemeEvent:^(NSString *payload, TCGBError *error) {
           NSLog(@"SchemeEvent occurred : %@", payload);
      }];
   }

*/
+ (void)showImageNoticesWithViewController:(nullable UIViewController *)viewController
                             configuration:(nullable TCGBImageNoticeConfiguration *)configuration
                           closeCompletion:(void(^ _Nullable)(TCGBError * _Nullable error))closeCompletion
                               schemeEvent:(void(^ _Nullable)(NSString * _Nullable payload, TCGBError * _Nullable error))schemeEvent;

/**---------------------------------------------------------------------------------------
 * @name Request to close Image Notices
 *  ---------------------------------------------------------------------------------------
 */

/** This is the method to request to close image notices.
 
 Request to close Image Notices.
 
 @param viewController Represent to current viewController.
 @warning If viewController is nil, it will be set to top most view controller automatically.

 
 ### Usage Example
 
    - (void)closeImageNotices {
        [TCGBImageNotice closeImageNoticesWithViewController:self];
    }
 
 */
+ (void)closeImageNoticesWithViewController:(nullable UIViewController *)viewController;

@end

NS_ASSUME_NONNULL_END

#endif /* TCGBImageNotice_h */
