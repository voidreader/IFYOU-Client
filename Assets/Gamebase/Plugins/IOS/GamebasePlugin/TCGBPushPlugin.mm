#import "TCGBPushPlugin.h"
#import "DelegateManager.h"
#import <Gamebase/Gamebase.h>
#import "TCGBJsonUtil.h"
#import "NativeMessage.h"
#import "TCGBPluginData.h"
#import "EngineMessage.h"

#define PUSH_API_REGISTER_PUSH              @"gamebase://registerPush"
#define PUSH_API_QUERY_PUSH                 @"gamebase://queryPush"
#define PUSH_API_SET_SANDBOX_MODE           @"gamebase://setSandboxMode"
#define PUSH_API_REGISTER_PUSH_WITH_OPTION  @"gamebase://registerPushWithOption"
#define PUSH_API_QUERY_TOKEN_INFO           @"gamebase://queryTokenInfo"
#define PUSH_API_GET_NOTIFICATION_OPTIONS   @"gamebase://getNotificationOptions"

@implementation TCGBPushPlugin

- (instancetype)init {
    if ((self = [super init]) == nil) {
        return nil;
    }
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:PUSH_API_REGISTER_PUSH target:self selector:@selector(registerPush:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:PUSH_API_QUERY_PUSH target:self selector:@selector(queryPush:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:PUSH_API_SET_SANDBOX_MODE target:self selector:@selector(setSandboxMode:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:PUSH_API_REGISTER_PUSH_WITH_OPTION target:self selector:@selector(registerPushWithOption:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:PUSH_API_QUERY_TOKEN_INFO target:self selector:@selector(queryTokenInfo:)];
    
    [[DelegateManager sharedDelegateManager] addSyncDelegate:PUSH_API_GET_NOTIFICATION_OPTIONS target:self selector:@selector(getNotificationOptions:)];
    
    return self;
}

-(void)registerPush:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    TCGBPushConfiguration * pushConfiguration = [TCGBPushConfiguration pushConfigurationWithJSONString:message.jsonData];
    
    [TCGBPush registerPushWithPushConfiguration:pushConfiguration completion:^(TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:nil extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);
    }];
}

-(void)queryPush:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    [TCGBPush queryPushWithCompletion:^(TCGBPushConfiguration *configuration, TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:[configuration JSONString] extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);
    }];
}

-(void)setSandboxMode:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    NSDictionary* convertedDic = [message.jsonData JSONDictionary];
    BOOL isSandbox = [convertedDic[@"isSandbox"] boolValue];
    [TCGBPush setSandboxMode:isSandbox];
}

-(void)registerPushWithOption:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    TCGBPushConfiguration * pushConfiguration = [TCGBPushConfiguration pushConfigurationWithJSONString:message.jsonData];
    TCGBNotificationOptions* notificationOptions = [TCGBNotificationOptions notificationOptionsWithJSONString:message.extraData];
    
    [TCGBPush registerPushWithPushConfiguration:pushConfiguration notificationOptions:notificationOptions completion:^(TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:nil extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);
    }];
}

-(void)queryTokenInfo:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    [TCGBPush queryTokenInfoWithCompletion:^(TCGBPushTokenInfo *tokenInfo, TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:[tokenInfo jsonString] extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);
    }];
}

-(NSString*)getNotificationOptions:(TCGBPluginData*)pluginData {
    TCGBNotificationOptions* notificationOptions = [TCGBPush notificationOptions];
    NSString* jsonString = @"";
    if(notificationOptions != nil){
        jsonString = [notificationOptions jsonString];
    }
    return jsonString;
}
@end

