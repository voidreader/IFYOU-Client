//
//  TCGBGamebaseEventHandler.h
//  Gamebase
//
//  Created by NHNEnt on 2020/05/14.
//  Copyright © 2020 NHN Corp. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "TCGBValueObject.h"
#import "TCGBConstants.h"

NS_ASSUME_NONNULL_BEGIN

#pragma mark - TCGBGamebaseEventMessage VO
@interface TCGBGamebaseEventMessage : NSObject <TCGBValueObject>

@property (nonatomic, strong)  NSString* category;
@property (nonatomic, strong, nullable) NSString* data;

+ (TCGBGamebaseEventMessage *)gamebaseEventMessageWithCategory:(TCGBGamebaseEventCategory)category data:(NSString * _Nullable)data;
+ (TCGBGamebaseEventMessage *)gamebaseEventMessageFromJsonString:(NSString * _Nullable)jsonString;

@end


#pragma mark - TCGBGamebaseEventServerPushData VO
@interface TCGBGamebaseEventServerPushData : NSObject <TCGBValueObject>

@property (nonatomic, strong, nullable) NSString* extras;

+ (TCGBGamebaseEventServerPushData *)gamebaseEventServerPushDataWithExtras:(NSString * _Nullable)extras;
+ (TCGBGamebaseEventServerPushData *)gamebaseEventServerPushDataFromJsonString:(NSString * _Nullable)jsonString;

@end


#pragma mark - TCGBGamebaseEventObserverData VO
@interface TCGBGamebaseEventObserverData : NSObject <TCGBValueObject>

@property (nonatomic, assign)           int64_t     code;
@property (nonatomic, strong, nullable) NSString*   message;
@property (nonatomic, strong, nullable) NSString*   extras;

+ (TCGBGamebaseEventObserverData *)gamebaseEventObserverDataWithCode:(int64_t)code message:(NSString * _Nullable)message extras:(NSString * _Nullable)extras;
+ (TCGBGamebaseEventObserverData *)gamebaseEventObserverDataFromJsonString:(nullable NSString *)jsonString;
@end

#pragma mark - TCGBPushMessage VO
@interface TCGBPushMessage : NSObject <TCGBValueObject>

@property (nonatomic, strong,) NSString* identifier;
@property (nonatomic, strong, nullable) NSString* title;
@property (nonatomic, strong, nullable) NSString* body;
@property (nonatomic, strong) NSString* extras;

+ (TCGBPushMessage *)pushMessageFromJsonString:(nullable NSString *)jsonString;

@end

#pragma mark - TCGBPushAction VO
@interface TCGBPushAction : NSObject <TCGBValueObject>

@property (nonatomic, strong) NSString* actionType;
@property (nonatomic, strong, nullable) TCGBPushMessage* message;
@property (nonatomic, strong, nullable) NSString* userText;

+ (TCGBPushAction *)pushActionFromJsonString:(nullable NSString *)jsonString;


@end

NS_ASSUME_NONNULL_END
