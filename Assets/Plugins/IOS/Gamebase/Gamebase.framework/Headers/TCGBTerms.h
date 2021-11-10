//
//  TCGBTerms.h
//  Gamebase
//
//  Created by NHN on 2021/01/06.
//  Copyright © 2021 NHN Corp. All rights reserved.
//

#ifndef TCGBTerms_h
#define TCGBTerms_h

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

@class TCGBQueryTermsResult;
@class TCGBUpdateTermsConfiguration;
@class TCGBWebSocket;
@class TCGBError;
@class TCGBDataContainer;

NS_ASSUME_NONNULL_BEGIN

@interface TCGBTerms : NSObject

/**---------------------------------------------------------------------------------------
 * @name Request Showing Terms View
 *  ---------------------------------------------------------------------------------------
 */

/** This is the method to request showing terms view. There is a viewController parameter and you may put your top most viewController. When finished showing terms view, this method calls completion.
 
 Request Showing terms view.
 
 @param viewController represent to current viewController
 @warning If viewController is nil, Terms view set it to top most view controller automatically.
 @param completion completion may call when finished showing terms view. This callback has TCGBDataContainer information.
 If there is an error, TCGBError will be returned.
 
 ### Usage Example
 
    - (void)showTermsView {
        [TCGBTerms showTermsViewWithViewController:self completion:^(TCGBDataContainer *dataContainer, TCGBError *error) {
            if (error) {
                dispatch_async(dispatch_get_main_queue(), ^{
                    NSLog(@"Error : %@", error);
                });
            } else {
                dispatch_async(dispatch_get_main_queue(), ^{
                    NSLog(@"Finish Terms View.");
                    if (dataContainer) {
                        // DataContainer can be converted to PushConfiguration.
                        TCGBPushConfiguration *configuration = [TCGBPushConfiguration fromDataContainer:dataContainer];
                    }
                });
            }
        }];
    }
 */
+ (void)showTermsViewWithViewController:(nullable UIViewController *)viewController
                             completion:(nullable void (^)(TCGBDataContainer * _Nullable dataContainer, TCGBError * _Nullable error))completion;


/** This is the method to query terms result to Gamebase Server. There is a viewController parameter and you may put your top most viewController.
 
 Query terms result to Gamebase Server.
 
 @param viewController represent to current viewController
 @param completion This callback has TCGBQueryTermsResult information.
 
 @see TCGBQueryTermsResult
 
 ### Usage Example
 
    - (void)queryTerms {
        [TCGBTerms queryTermsWithViewController:self completion:^(TCGBQueryTermsResult *queryTermsResult, TCGBError *error) {
            if (error) {
                dispatch_async(dispatch_get_main_queue(), ^{
                    NSLog(@"Error : %@", error);
                });
            } else {
                dispatch_async(dispatch_get_main_queue(), ^{
                    int termsSeq = queryTermsResult.termsSeq;
                    NSString *termsVersion = queryTermsResult.termsVersion;
                    ...
                });
            }
        }];
    }
 */
+ (void)queryTermsWithViewController:(nullable UIViewController *)viewController
                          completion:(void (^)(TCGBQueryTermsResult * _Nullable queryTermsResult, TCGBError * _Nullable error))completion;


/** This is the method to update terms information to Gamebase Server. There is a viewController parameter and you may put your top most viewController.
 
 Update terms information to Gamebase Server.
 
 @param viewController represent to current viewController
 @param configuration The configuration which has user's terms agreed information.
 @param completion completion may call when finished update terms information.
 If there is an error, TCGBError will be returned.
 
 @see TCGBUpdateTermsConfiguration
 
 ### Usage Example
 
    - (void)updateTerms {
        TCGBTermsContent *termsContent = [TCGBTermsContent termsContentWithTermsContentSeq:12 agreed:YES];

        NSMutableArray *contents = [NSMutableArray array];
        [contents addObject:termsContent];

        TCGBUpdateTermsConfiguration *configuration = [TCGBUpdateTermsConfiguration configurationWithTermsVersion:@"1.2.3" termsSeq:1 contents:contents];
 
        [TCGBTerms updateTermsWithViewController:self configuration:configuration completion:^(TCGBError *error) {
            if (error) {
                dispatch_async(dispatch_get_main_queue(), ^{
                    NSLog(@"Error : %@", error);
                });
            } else {
                dispatch_async(dispatch_get_main_queue(), ^{
                    NSLog(@"Update Terms Success");
                });
            }
        }];
    }
 */
+ (void)updateTermsWithViewController:(nullable UIViewController *)viewController
                        configuration:(TCGBUpdateTermsConfiguration *)configuration
                           completion:(nullable void (^)(TCGBError * _Nullable error))completion;

@end

NS_ASSUME_NONNULL_END

#endif /* TCGBTerms_h */
