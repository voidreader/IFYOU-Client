//
//  TCGBContact.h
//  Gamebase
//
//  Created by NHNEnt on 21/08/2019.
//  Copyright © 2019 NHN Corp. All rights reserved.
//

#ifndef TCGBContact_h
#define TCGBContact_h

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

NS_ASSUME_NONNULL_BEGIN

@class TCGBContactConfiguration;
@class TCGBError;

@interface TCGBContact : NSObject

/**---------------------------------------------------------------------------------------
* @name Request Showing Online Contact
*  ---------------------------------------------------------------------------------------
*/

/** This is the method to request showing online contact. There is a viewController parameter and you may put your top most viewController. When closed showing online contact, this method calls completion.

Request Showing Online Contact.

@param viewController  represent to current viewcontroller.
@param completion      completion may call when finished showing online contact.
If there is an error, TCGBError will be returned.

### Usage Example

   - (void)showOnlineContact {
       [TCGBContact openContactWithViewController:self completion:^(TCGBError *error) {
           if (error) {
               dispatch_async(dispatch_get_main_queue(), ^{
                   NSLog(@"Failed to open online contact. Error : %@", error);
               });
           } else {
               dispatch_async(dispatch_get_main_queue(), ^{
                   NSLog(@"Closed online contact.");
               });
           }
       }];
   }

*/

+ (void)openContactWithViewController:(nullable UIViewController *)viewController
                           completion:(nullable void(^)(TCGBError * _Nullable error))completion;

/** This is the method to request showing online contact. There is a viewController parameter and you may put your top most viewController. When closed showing online contact, this method calls completion.

Request Showing Online Contact.

@param viewController  represent to current viewcontroller.
@param configuration This configuration includes userName.
@param completion      completion may call when finished showing online contact.
If there is an error, TCGBError will be returned.

### Usage Example

   - (void)showOnlineContact {
       TCGBContactConfiguration *configuration = [TCGBContactConfiguration contactConfigurationWithUserName:@"USER_NAME"];
       [TCGBContact openContactWithViewController:self configuration:configuration completion:^(TCGBError *error) {
           if (error) {
               dispatch_async(dispatch_get_main_queue(), ^{
                   NSLog(@"Failed to open online contact. Error : %@", error);
               });
           } else {
               dispatch_async(dispatch_get_main_queue(), ^{
                   NSLog(@"Closed online contact.");
               });
           }
       }];
   }

*/
+ (void)openContactWithViewController:(nullable UIViewController *)viewController
                        configuration:(nullable TCGBContactConfiguration *)configuration
                           completion:(nullable void(^)(TCGBError * _Nullable error))completion;


/**---------------------------------------------------------------------------------------
* @name Request Online Contact URL
*  ---------------------------------------------------------------------------------------
*/

/** This is the method to request online contact url.

Request Online Contact URL.

@param completion      completion may return online contact url.
If there is an error, TCGBError will be returned.

### Usage Example

   - (void)showOnlineContact {
       [TCGBContact requestContactURLWithCompletion:^(NSString *contactUrl, TCGBError *error) {
           if (error) {
               dispatch_async(dispatch_get_main_queue(), ^{
                   NSLog(@"Failed to request online contact url. Error : %@", error);
               });
           } else {
               dispatch_async(dispatch_get_main_queue(), ^{
                   NSLog(@"Online contact url : %@", contactUrl");
               });
           }
       }];
   }

*/
+ (void)requestContactURLWithCompletion:(void(^)(NSString * _Nullable contactUrl, TCGBError * _Nullable error))completion;

/** This is the method to request online contact url.

Request Online Contact URL.
 
@param configuration      This configuration includes userName.
@param completion      completion may return online contact url.
If there is an error, TCGBError will be returned.

### Usage Example

   - (void)showOnlineContact {
       TCGBContactConfiguration *configuration = [TCGBContactConfiguration contactConfigurationWithUserName:@"USER_NAME"];
       [TCGBContact requestContactURLWithConfiguration:configuration completion:^(NSString *contactUrl, TCGBError *error) {
           if (error) {
               dispatch_async(dispatch_get_main_queue(), ^{
                   NSLog(@"Failed to request online contact url. Error : %@", error);
               });
           } else {
               dispatch_async(dispatch_get_main_queue(), ^{
                   NSLog(@"Online contact url : %@", contactUrl");
               });
           }
       }];
   }

*/
+ (void)requestContactURLWithConfiguration:(nullable TCGBContactConfiguration *)configuration
                                completion:(void(^)(NSString * _Nullable contactUrl, TCGBError * _Nullable error))completion;

@end

NS_ASSUME_NONNULL_END

#endif /* TCGBContact_h */
