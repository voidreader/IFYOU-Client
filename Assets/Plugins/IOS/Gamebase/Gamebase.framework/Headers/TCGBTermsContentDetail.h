//
//  TCGBTermsContentDetail.h
//  Gamebase
//
//  Created by NHN on 2021/01/12.
//  Copyright © 2021 NHN Corp. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "TCGBValueObject.h"

NS_ASSUME_NONNULL_BEGIN

@interface TCGBTermsContentDetail : NSObject

/**---------------------------------------------------------------------------------------
 * @name Properties
 *  ---------------------------------------------------------------------------------------
 */

/**
 Terms item key
 */
@property (assign) int termsContentSeq;

/**
 Terms item name
 */
@property (nonatomic, strong) NSString *name;

/**
 Whether terms item is required
 */
@property (assign) BOOL required;

/**
 Whether to AD push ("NONE", "ALL", "DAY", "NIGHT")
 */
@property (nonatomic, strong) NSString *agreePush;

/**
 Whether the user agrees to terms item
 */
@property (assign) BOOL agreed;

/**
 Step 1 Item Exposure Order
 */
@property (assign) int node1DepthPosition;

/**
 Step 2 Item Exposure Order
 
 @warning If it is does not exist, the value is -1.
 */
@property (assign) int node2DepthPosition;

/**
 Terms item detail URL
 */
@property (nonatomic, strong, nullable) NSString *detailPageUrl;

/**---------------------------------------------------------------------------------------
 * @name Initialization
 *  ---------------------------------------------------------------------------------------
 */

/**
 Creates a TCGBTermsContentDetail instance with several properties.
 
 @param termsContentSeq Terms item key
 @param name Terms item name
 @param required Whether terms item is required
 @param agreePush Whether to AD push
 @param agreed Whether the user agrees to terms item
 @param node1DepthPosition Step 1 Item Exposure Order
 @param node2DepthPosition Step 2 Item Exposure Order
 @param detailPageUrl Terms item detail URL
 */
+ (instancetype)termsContentDetailWithTermsContentSeq:(int)termsContentSeq
                                                 name:(NSString *)name
                                             required:(BOOL)required
                                            agreePush:(NSString *)agreePush
                                               agreed:(BOOL)agreed
                                   node1DepthPosition:(int)node1DepthPosition
                                   node2DepthPosition:(int)node2DepthPosition
                                        detailPageUrl:(nullable NSString *)detailPageUrl;

/**
 Creates a TCGBTermsContentDetail instance with several properties.
 
 @param result This dictionary must have keys such as `termsContentSeq`, `name`, etc.
 */
+ (instancetype)termsContentDetailWithDictionary:(NSDictionary *)result;

- (NSString *)jsonString;
- (NSString *)prettyJsonString;
- (NSDictionary *)dictionaryWithTermsContentDetail;

@end

NS_ASSUME_NONNULL_END
