#import "TCGBContactPlugin.h"
#import "DelegateManager.h"
#import <Gamebase/Gamebase.h>
#import "TCGBJsonUtil.h"
#import "NativeMessage.h"
#import "TCGBPluginData.h"
#import "EngineMessage.h"
#import "TCGBViewControllerManager.h"

#define CONTACT_API_OPEN_CONTACT                            @"gamebase://openContact"
#define CONTACT_API_OPEN_CONTACT_WITH_CONFIGURATION         @"gamebase://openContactWithConfiguration"
#define CONTACT_API_REQUEST_CONTACT_URL                     @"gamebase://requestContactURL"
#define CONTACT_API_REQUEST_CONTACT_URL_WITH_CONFIGURATION  @"gamebase://requestContactURLWithConfiguration"

@implementation TCGBContactPlugin

- (instancetype)init {
    if ((self = [super init]) == nil) {
        return nil;
    }
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:CONTACT_API_OPEN_CONTACT target:self selector:@selector(openContact:)];
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:CONTACT_API_OPEN_CONTACT_WITH_CONFIGURATION target:self selector:@selector(openContactWithConfiguration:)];
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:CONTACT_API_REQUEST_CONTACT_URL target:self selector:@selector(requestContactURL:)];
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:CONTACT_API_REQUEST_CONTACT_URL_WITH_CONFIGURATION target:self selector:@selector(requestContactURLWithConfiguration:)];
    
    return self;
}

-(void)openContact:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    [TCGBContact openContactWithViewController:[[TCGBViewControllerManager sharedGamebaseViewControllerManager] getViewController] completion:^(TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:nil extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);
    }];
}

-(void)openContactWithConfiguration:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    TCGBContactConfiguration* configuration = [TCGBContactConfiguration contactConfigurationWithJSONString:message.jsonData];
    
    [TCGBContact openContactWithViewController:[[TCGBViewControllerManager sharedGamebaseViewControllerManager] getViewController] configuration:configuration completion:^(TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:nil extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);
    }];
}

-(void)requestContactURL:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    [TCGBContact requestContactURLWithCompletion:^(NSString *data, TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:data extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);
    }];
}

-(void)requestContactURLWithConfiguration:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    TCGBContactConfiguration* configuration = [TCGBContactConfiguration contactConfigurationWithJSONString:message.jsonData];
    
    [TCGBContact requestContactURLWithConfiguration:configuration completion:^(NSString *data, TCGBError *error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:data extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);
    }];
}
@end

