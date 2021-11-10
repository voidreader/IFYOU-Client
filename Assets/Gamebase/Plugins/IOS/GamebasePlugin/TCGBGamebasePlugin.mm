#import "TCGBGamebasePlugin.h"
#import "DelegateManager.h"
#import <Gamebase/Gamebase.h>
#import "TCGBJsonUtil.h"
#import "NativeMessage.h"
#import "EngineMessage.h"

#define GAMEBASE_API_INITIALIZE                     @"gamebase://initialize"
#define GAMEBASE_API_SET_DEBUG_MODE                 @"gamebase://setDebugMode"
#define GAMEBASE_API_GET_SDK_VERSION                @"gamebase://getSDKVersion"
#define GAMEBASE_API_GET_USERID                     @"gamebase://getUserID"
#define GAMEBASE_API_GET_ACCESSTOKEN                @"gamebase://getAccessToken"
#define GAMEBASE_API_GET_LAST_LOGGED_IN_PROVIDER    @"gamebase://getLastLoggedInProvider"
#define GAMEBASE_API_GET_DEVICE_LANGUAGE_CODE       @"gamebase://getDeviceLanguageCode"
#define GAMEBASE_API_GET_CARRIER_CODE               @"gamebase://getCarrierCode"
#define GAMEBASE_API_GET_CARRIER_NAME               @"gamebase://getCarrierName"
#define GAMEBASE_API_GET_COUNTRY_CODE               @"gamebase://getCountryCode"
#define GAMEBASE_API_GET_COUNTRY_CODE_OF_USIM       @"gamebase://getCountryCodeOfUSIM"
#define GAMEBASE_API_GET_COUNTRY_CODE_OF_DEVICE     @"gamebase://getCountryCodeOfDevice"
#define GAMEBASE_API_IS_SANDBOX                     @"gamebase://isSandbox"
#define GAMEBASE_API_SET_DISPLAY_LANGUAGE_CODE      @"gamebase://setDisplayLanguageCode"
#define GAMEBASE_API_GET_DISPLAY_LANGUAGE_CODE      @"gamebase://getDisplayLanguageCode"
#define GAMEBASE_API_ADD_SERVER_PUSH_EVENT          @"gamebase://addServerPushEvent"
#define GAMEBASE_API_REMOVE_SERVER_PUSH_EVENT       @"gamebase://removeServerPushEvent"
#define GAMEBASE_API_ADD_OBSERVER                   @"gamebase://addObserver"
#define GAMEBASE_API_REMOVE_OBSERVER                @"gamebase://removeObserver"
#define GAMEBASE_API_ADD_EVENT_HANDLER              @"gamebase://addEventHandler"
#define GAMEBASE_API_REMOVE_EVENT_HANDLER           @"gamebase://removeEventHandler"

@implementation TCGBGamebasePlugin

@synthesize observerMessage                         = _observerMessage;
@synthesize observerPluginData                      = _observerPluginData;

@synthesize serverPushMessage                       = _serverPushMessage;
@synthesize serverPushEventPluginData               = _serverPushEventPluginData;

@synthesize eventHandlerMessage                     = _eventHandlerMessage;
@synthesize eventHandlerPluginData                   = _eventHandlerPluginData;

- (instancetype)init {
    if ((self = [super init]) == nil) {
        return nil;
    }
    
    __block TCGBGamebasePlugin *tempSelf = self;
    
    _serverPushMessage = ^(TCGBServerPushMessage* message){
        NSMutableDictionary* jsonDic = [[NSMutableDictionary alloc]init];
        jsonDic[@"type"] = message.type;
        jsonDic[@"data"] = message.data;
        
        EngineMessage* engineMessage = [[EngineMessage alloc]initWithJsonString:tempSelf.serverPushEventPluginData.jsonData];
        
        NSString* jsonString = [jsonDic JSONString];
        
        NativeMessage* responseMessage = [[NativeMessage alloc] initWithMessage:GAMEBASE_API_ADD_SERVER_PUSH_EVENT handle:engineMessage.handle TCGBError:nil jsonData:jsonString extraData:nil];
        
        tempSelf.serverPushEventPluginData.completion(tempSelf.serverPushEventPluginData.jsonData, responseMessage);
    };
    
    _observerMessage = ^(TCGBObserverMessage* message){
        NSMutableDictionary* jsonDic = [[NSMutableDictionary alloc] init];
        jsonDic[@"type"] = message.type;
        jsonDic[@"data"] = message.data;
        
        EngineMessage* engineMessage = [[EngineMessage alloc]initWithJsonString:tempSelf.observerPluginData.jsonData];
        
        NSString* jsonString = [jsonDic JSONString];
        
        NativeMessage* responseMessage = [[NativeMessage alloc] initWithMessage:GAMEBASE_API_ADD_OBSERVER handle:engineMessage.handle TCGBError:nil jsonData:jsonString extraData:nil];
        
        tempSelf.observerPluginData.completion(tempSelf.observerPluginData.jsonData, responseMessage);
    };
    
    _eventHandlerMessage = ^(TCGBGamebaseEventMessage* message){
        NSMutableDictionary* jsonDic = [[NSMutableDictionary alloc] init];
        jsonDic[@"category"] = message.category;
        jsonDic[@"data"] = message.data;
        
        EngineMessage* engineMessage = [[EngineMessage alloc]initWithJsonString:tempSelf.eventHandlerPluginData.jsonData];
        
        NSString* jsonString = [jsonDic JSONString];
        
        NativeMessage* responseMessage = [[NativeMessage alloc] initWithMessage:GAMEBASE_API_ADD_EVENT_HANDLER handle:engineMessage.handle TCGBError:nil jsonData:jsonString extraData:nil];
        
        tempSelf.eventHandlerPluginData.completion(tempSelf.eventHandlerPluginData.jsonData, responseMessage);
    };
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:GAMEBASE_API_INITIALIZE target:self selector:@selector(initialize:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:GAMEBASE_API_SET_DEBUG_MODE target:self selector:@selector(setDebugMode:)];
    
    [[DelegateManager sharedDelegateManager] addSyncDelegate:GAMEBASE_API_GET_SDK_VERSION target:self selector:@selector(getSDKVersion:)];
    
    [[DelegateManager sharedDelegateManager] addSyncDelegate:GAMEBASE_API_GET_USERID target:self selector:@selector(getUserID:)];
    
    [[DelegateManager sharedDelegateManager] addSyncDelegate:GAMEBASE_API_GET_ACCESSTOKEN target:self selector:@selector(getAccessToken:)];
    
    [[DelegateManager sharedDelegateManager] addSyncDelegate:GAMEBASE_API_GET_LAST_LOGGED_IN_PROVIDER target:self selector:@selector(getLastLoggedInProvider:)];
    
    [[DelegateManager sharedDelegateManager] addSyncDelegate:GAMEBASE_API_GET_DEVICE_LANGUAGE_CODE target:self selector:@selector(getDeviceLanguageCode:)];
    
    [[DelegateManager sharedDelegateManager] addSyncDelegate:GAMEBASE_API_GET_CARRIER_CODE target:self selector:@selector(getCarrierCode:)];
    
    [[DelegateManager sharedDelegateManager] addSyncDelegate:GAMEBASE_API_GET_CARRIER_NAME target:self selector:@selector(getCarrierName:)];
    
    [[DelegateManager sharedDelegateManager] addSyncDelegate:GAMEBASE_API_GET_COUNTRY_CODE target:self selector:@selector(getCountryCode:)];
    
    [[DelegateManager sharedDelegateManager] addSyncDelegate:GAMEBASE_API_GET_COUNTRY_CODE_OF_USIM target:self selector:@selector(getCountryCodeOfUSIM:)];
    
    [[DelegateManager sharedDelegateManager] addSyncDelegate:GAMEBASE_API_GET_COUNTRY_CODE_OF_DEVICE target:self selector:@selector(getCountryCodeOfDevice:)];
    
    [[DelegateManager sharedDelegateManager] addSyncDelegate:GAMEBASE_API_IS_SANDBOX target:self selector:@selector(isSandbox:)];
    
    [[DelegateManager sharedDelegateManager] addSyncDelegate:GAMEBASE_API_SET_DISPLAY_LANGUAGE_CODE target:self selector:@selector(setDisplayLanguageCode:)];
    
    [[DelegateManager sharedDelegateManager] addSyncDelegate:GAMEBASE_API_GET_DISPLAY_LANGUAGE_CODE target:self selector:@selector(getDisplayLanguageCode:)];
    
    [[DelegateManager sharedDelegateManager] addSyncDelegate:GAMEBASE_API_ADD_SERVER_PUSH_EVENT target:self selector:@selector(addServerPushEvent:)];
    
    [[DelegateManager sharedDelegateManager] addSyncDelegate:GAMEBASE_API_REMOVE_SERVER_PUSH_EVENT target:self selector:@selector(removeServerPushEvent:)];
    
    [[DelegateManager sharedDelegateManager] addSyncDelegate:GAMEBASE_API_ADD_OBSERVER target:self selector:@selector(addObserver:)];
    
    [[DelegateManager sharedDelegateManager] addSyncDelegate:GAMEBASE_API_REMOVE_OBSERVER target:self selector:@selector(removeObserver:)];
    
    [[DelegateManager sharedDelegateManager] addSyncDelegate:GAMEBASE_API_ADD_EVENT_HANDLER target:self selector:@selector(addEventHandler:)];
    
    [[DelegateManager sharedDelegateManager] addSyncDelegate:GAMEBASE_API_REMOVE_EVENT_HANDLER target:self selector:@selector(removeEventHandler:)];
    
    return self;
}

-(void)initialize:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    NSDictionary* convertedDic = [message.jsonData JSONDictionary];
    TCGBConfiguration* con = [TCGBConfiguration configurationWithAppID:convertedDic[@"appID"] appVersion:convertedDic[@"appVersion"] zoneType:convertedDic[@"zoneType"]];
    
    [con enablePopup:[convertedDic[@"enablePopup"] boolValue]];
    [con enableLaunchingStatusPopup:[convertedDic[@"enableLaunchingStatusPopup"] boolValue]];
    [con enableBanPopup:[convertedDic[@"enableBanPopup"] boolValue]];
    [con setStoreCode:convertedDic[@"storeCode"]];
    [con setDisplayLanguageCode:convertedDic[@"displayLanguageCode"]];
    
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wundeclared-selector"
    NSString* engineName = @"UNITY";
    if ([con respondsToSelector:@selector(setGameEngine:)] == YES) {
        [con performSelector:@selector(setGameEngine:) withObject:engineName];
    }
#pragma clang diagnostic pop
    
    [TCGBGamebase initializeWithConfiguration:con completion:^(id launchingData, TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:[launchingData JSONString] extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);
    }];
}

-(void)setDebugMode:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    NSDictionary* convertedDic = [message.jsonData JSONDictionary];
    BOOL isDebugMode = [convertedDic[@"isDebugMode"] boolValue];
    [TCGBGamebase setDebugMode:isDebugMode];
}

-(NSString*)getSDKVersion:(TCGBPluginData*)pluginData {
    NSString* version = [TCGBGamebase SDKVersion];
    return version;
}

-(NSString*)getUserID:(TCGBPluginData*)pluginData {
    return [TCGBGamebase userID];
}

-(NSString*)getAccessToken:(TCGBPluginData*)pluginData {
    return [TCGBGamebase accessToken];
}

-(NSString*)getLastLoggedInProvider:(TCGBPluginData*)pluginData {
    return [TCGBGamebase lastLoggedInProvider];
}

-(NSString*)getDeviceLanguageCode:(TCGBPluginData*)pluginData {
    return [TCGBGamebase deviceLanguageCode];
}

-(NSString*)getCarrierCode:(TCGBPluginData*)pluginData {
    return [TCGBGamebase carrierCode];
}

-(NSString*)getCarrierName:(TCGBPluginData*)pluginData {
    return [TCGBGamebase carrierName];
}

-(NSString*)getCountryCode:(TCGBPluginData*)pluginData {
    return [TCGBGamebase countryCode];
}

-(NSString*)getCountryCodeOfUSIM:(TCGBPluginData*)pluginData {
    return [TCGBGamebase countryCodeOfUSIM];
}

-(NSString*)getCountryCodeOfDevice:(TCGBPluginData*)pluginData {
    return [TCGBGamebase countryCodeOfDevice];
}

-(NSString*)isSandbox:(TCGBPluginData*)pluginData {
    NSMutableDictionary *contentDictionary = [[NSMutableDictionary alloc]init];
    [contentDictionary setValue:[NSNumber numberWithBool:[TCGBGamebase isSandbox]] forKey:@"isSandbox"];
    
    NSString* returnValue = [contentDictionary JSONString];
    if(returnValue == nil)
        return @"";
    
    return returnValue ;
}

-(NSString*)setDisplayLanguageCode:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    [TCGBGamebase setDisplayLanguageCode:message.jsonData];
    return @"";
}

-(NSString*)getDisplayLanguageCode:(TCGBPluginData*)pluginData {
    return [TCGBGamebase displayLanguageCode];
}

-(NSString*)addServerPushEvent:(TCGBPluginData*)pluginData {
    _serverPushEventPluginData = pluginData;
    
    [TCGBGamebase addServerPushEvent:_serverPushMessage];
    return @"";
}

-(NSString*)removeServerPushEvent:(TCGBPluginData*)pluginData{
    _serverPushEventPluginData = nil;
    
    [TCGBGamebase removeServerPushEvent:_serverPushMessage];
    return @"";
}

-(NSString*)addObserver:(TCGBPluginData*)pluginData{
    _observerPluginData =pluginData;
    
    [TCGBGamebase addObserver:_observerMessage];
    return @"";
}

-(NSString*)removeObserver:(TCGBPluginData*)pluginData{
    _observerPluginData = nil;
    
    [TCGBGamebase removeObserver:_observerMessage];
    return @"";
}

-(NSString*)addEventHandler:(TCGBPluginData*)pluginData{
    _eventHandlerPluginData =pluginData;
    
    [TCGBGamebase addEventHandler:_eventHandlerMessage];
    return @"";
}

-(NSString*)removeEventHandler:(TCGBPluginData*)pluginData{
    _eventHandlerPluginData = nil;
    
    [TCGBGamebase removeEventHandler:_eventHandlerMessage];
    return @"";
}
@end

