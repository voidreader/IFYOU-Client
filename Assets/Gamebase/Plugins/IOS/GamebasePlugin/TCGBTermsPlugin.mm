#import "TCGBTermsPlugin.h"
#import "DelegateManager.h"
#import <Gamebase/Gamebase.h>
#import "NativeMessage.h"
#import "TCGBJsonUtil.h"
#import "TCGBPluginData.h"
#import "EngineMessage.h"
#import "TCGBViewControllerManager.h"

#define TERMS_API_SHOW_TERMS_VIEW   @"gamebase://showTermsView"
#define TERMS_API_UPDATE_TERMS      @"gamebase://updateTerms"
#define TERMS_API_QUERY_TERMS       @"gamebase://queryTerms"

@implementation TCGBTermsPlugin

- (instancetype)init {
    if ((self = [super init]) == nil) {
        return nil;
    }
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:TERMS_API_SHOW_TERMS_VIEW target:self selector:@selector(showTermsView:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:TERMS_API_UPDATE_TERMS target:self selector:@selector(updateTerms:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:TERMS_API_QUERY_TERMS target:self selector:@selector(queryTerms:)];
    
    return self;
}

-(void)showTermsView:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    [TCGBTerms showTermsViewWithViewController:[[TCGBViewControllerManager sharedGamebaseViewControllerManager] getViewController] completion:^(TCGBDataContainer * _Nullable dataContainer, TCGBError * _Nullable error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:[dataContainer jsonString] extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);
    }];
}

-(void)updateTerms:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    NSDictionary* configuration = [message.jsonData JSONDictionary];
    NSDictionary *contentsDict = ((NSArray *)configuration[@"contents"])[0];
    NSMutableArray *contents = [NSMutableArray array];
    [contents addObject:[TCGBTermsContent termsContentWithDictionary:contentsDict]];
    NSString* termsVersion   = configuration[@"termsVersion"];
    int termsSeq   = [configuration[@"termsSeq"] intValue];

    TCGBUpdateTermsConfiguration* updateTermsConfiguration = [TCGBUpdateTermsConfiguration updateTermsConfigurationWithTermsVersion:termsVersion termsSeq:termsSeq contents:contents];
    
    [TCGBTerms updateTermsWithViewController:[[TCGBViewControllerManager sharedGamebaseViewControllerManager] getViewController] configuration:updateTermsConfiguration completion:^(TCGBError * _Nullable error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:nil extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);
    }];
}

-(void)queryTerms:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    [TCGBTerms queryTermsWithViewController:[[TCGBViewControllerManager sharedGamebaseViewControllerManager] getViewController] completion:^(TCGBQueryTermsResult * _Nullable queryTermsResult, TCGBError * _Nullable error) {
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:[queryTermsResult jsonString] extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);
    }];
}
@end

