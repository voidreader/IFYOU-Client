#import "TCGBImageNoticePlugin.h"
#import "DelegateManager.h"
#import <Gamebase/Gamebase.h>
#import "NativeMessage.h"
#import "TCGBJsonUtil.h"
#import "TCGBPluginData.h"
#import "EngineMessage.h"
#import "TCGBViewControllerManager.h"

#define IMAGE_NOTICE_API_SHOW_IMAGE_NOTICES     @"gamebase://showImageNotices"
#define IMAGE_NOTICE_API_CLOSE_IMAGE_NOTICES    @"gamebase://closeImageNotices"
#define IMAGE_NOTICE_API_SCHEME_EVENT           @"gamebase://schemeEventImageNotices"

@implementation TCGBImageNoticePlugin

- (instancetype)init {
    if ((self = [super init]) == nil) {
        return nil;
    }    
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:IMAGE_NOTICE_API_SHOW_IMAGE_NOTICES target:self selector:@selector(showImageNotices:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:IMAGE_NOTICE_API_CLOSE_IMAGE_NOTICES target:self selector:@selector(closeImageNotices:)];    
    
    return self;
}

-(void)showImageNotices:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
	
    NSDictionary* extraDataDic = [message.extraData JSONDictionary];    
    int schemeEvent = [extraDataDic[@"schemeEvent"] intValue];
    
    TCGBImageNoticeConfiguration* conf = nil;
    
    NSDictionary* configuration            = [message.jsonData JSONDictionary];
    if(nil != configuration && [configuration isEqual:[NSNull null]] == NO) {
        CGFloat colorR                      = (float)[configuration[@"colorR"] intValue]/(float)255;
        CGFloat colorG                      = (float)[configuration[@"colorG"] intValue]/(float)255;
        CGFloat colorB                      = (float)[configuration[@"colorB"] intValue]/(float)255;
        CGFloat colorA                      = (float)[configuration[@"colorA"] intValue]/(float)255;
        NSInteger timeout                   = [configuration[@"timeout"] integerValue];
        
        conf = [[TCGBImageNoticeConfiguration alloc] init];
        conf.backgroundColor = [UIColor colorWithRed:colorR green:colorG blue:colorB alpha:colorA];
        conf.timeoutMS = timeout;
    }
    
    [TCGBImageNotice showImageNoticesWithViewController:[[TCGBViewControllerManager sharedGamebaseViewControllerManager] getViewController] configuration:conf closeCompletion:^(TCGBError *error){
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:nil extraData:[NSString stringWithFormat:@"%d", schemeEvent]];
        pluginData.completion(pluginData.jsonData, responseMessage);
    } schemeEvent:^(NSString *payload, TCGBError *error){
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:IMAGE_NOTICE_API_SCHEME_EVENT handle:schemeEvent TCGBError:error jsonData:payload extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);
    }];
}

-(void)closeImageNotices:(TCGBPluginData*)pluginData {
    [TCGBImageNotice closeImageNoticesWithViewController:[[TCGBViewControllerManager sharedGamebaseViewControllerManager] getViewController]];
}
@end

