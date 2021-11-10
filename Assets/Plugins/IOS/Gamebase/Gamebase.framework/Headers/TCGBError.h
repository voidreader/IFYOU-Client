//
//  TCGBError.h
//  Gamebase
//
//  Created by NHN on 2016. 6. 20..
//  © NHN Corp. All rights reserved.
//
#ifndef TCGBError_h
#define TCGBError_h

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

extern NSString* const TCGBDomainGamebase;
extern NSString* const TCGBErrorUserInfoTransactionId;

/** TCGBError class represents a result of some APIs or an occurred error.
 */
@interface TCGBError : NSError

/**---------------------------------------------------------------------------------------
 * @name Initialization
 *  ---------------------------------------------------------------------------------------
 */

/**
 Creates TCGBError instance.
 
 @param code error code
*/
+ (TCGBError *)resultWithCode:(NSInteger)code;

/**
 Creates TCGBError instance.
 
 @param code error code.
 @param message error message.
 */
+ (TCGBError *)resultWithCode:(NSInteger)code message:(NSString *)message;

/**
 Creates TCGBError instance.
 
 @param domain error domain.
 @param code error code.
 @param userInfo a dictionary with userInfo.
 */
+ (TCGBError *)resultWithDomain:(NSString *)domain code:(NSInteger)code userInfo:(nullable NSDictionary *)userInfo;

/**
 Create TCGBError instance. If the description value is nil or empty string, it will be set a value to default error message.
 
 @param domain domain error domain.
 @param code error code.
 @param description description about error. If it's value is set to nil or empty.
 @param underlyingError error object what a cause of error.
 */
+ (TCGBError *)resultWithDomain:(NSString *)domain code:(NSInteger)code description:(nullable NSString *)description underlyingError:(nullable NSError *)underlyingError;

+ (NSError *)errorWithError:(NSError *)error transactionId:(NSString *)transactionId;

/**---------------------------------------------------------------------------------------
 * @name Properties
 *  ---------------------------------------------------------------------------------------
 */

/**
 Error message
 */
- (NSString *)message;

/**
 Pretty JSON string
 */
- (NSString *)prettyJsonString;

/**
 JSON string
 */
- (NSString *)jsonString;

@end

NS_ASSUME_NONNULL_END

#endif /* TCGBError_h */
