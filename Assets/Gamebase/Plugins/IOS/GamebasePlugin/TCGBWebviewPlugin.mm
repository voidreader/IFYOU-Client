#import "TCGBWebviewPlugin.h"
#import "DelegateManager.h"
#import <Gamebase/Gamebase.h>
#import "NativeMessage.h"
#import "TCGBJsonUtil.h"
#import "TCGBPluginData.h"
#import "EngineMessage.h"
#import "TCGBViewControllerManager.h"

#define WEBVIEW_API_OPEN_WEBBROWSER                 @"gamebase://openWebBrowser"
#define WEBVIEW_API_SHOW_WEBVIEW                 @"gamebase://showWebView"
#define WEBVIEW_API_CLOSE_WEBVIEW                 @"gamebase://closeWebView"
#define WEBVIEW_API_SCHEME_EVENT                 @"gamebase://schemeEvent"

@implementation TCGBWebviewPlugin

- (instancetype)init {
    if ((self = [super init]) == nil) {
        return nil;
    }    
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:WEBVIEW_API_OPEN_WEBBROWSER target:self selector:@selector(openWebBrowser:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:WEBVIEW_API_SHOW_WEBVIEW target:self selector:@selector(showWebview:)];
    
    [[DelegateManager sharedDelegateManager] addAsyncDelegate:WEBVIEW_API_CLOSE_WEBVIEW target:self selector:@selector(closeWebview:)];
    
    return self;
}

-(void)openWebBrowser:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    [TCGBWebView openWebBrowserWithURL:message.jsonData];
}

-(void)showWebview:(TCGBPluginData*)pluginData {
    EngineMessage* message = [[EngineMessage alloc]initWithJsonString:pluginData.jsonData];
    
    NSDictionary* extraDataDic = [message.extraData JSONDictionary];
    NSArray* schemeArray = (NSArray*)extraDataDic[@"schemeList"];
    int schemeEvent = [extraDataDic[@"schemeEvent"] intValue];
    
    NSDictionary* convertedDic = [message.jsonData JSONDictionary];
    NSString* url                       = convertedDic[@"url"];
    TCGBWebViewConfiguration* conf = nil;
    
    NSDictionary* configuration            = convertedDic[@"configuration"];
    if(nil != configuration && [configuration isEqual:[NSNull null]] == NO) {
        NSString* title                     = configuration[@"title"];
        NSInteger orientation               = [configuration[@"orientation"] integerValue];
        CGFloat colorR                      = (float)[configuration[@"colorR"] intValue]/(float)255;
        CGFloat colorG                      = (float)[configuration[@"colorG"] intValue]/(float)255;
        CGFloat colorB                      = (float)[configuration[@"colorB"] intValue]/(float)255;
        CGFloat colorA                      = (float)[configuration[@"colorA"] intValue]/(float)255;
        NSInteger barHeight                 = [configuration[@"barHeight"] integerValue];
        bool buttonVisible                  = [configuration[@"buttonVisible"] boolValue];
        NSString* backButtonImageResource   = configuration[@"backButtonImageResource"];
        NSString* closeButtonImageResource  = configuration[@"closeButtonImageResource"];
        NSInteger contentMode               = [configuration[@"contentMode"] integerValue];
        
        conf = [[TCGBWebViewConfiguration alloc] init];
        conf.navigationBarTitle = title;
        conf.orientationMask = orientation;
        conf.navigationBarColor = [UIColor colorWithRed:colorR green:colorG blue:colorB alpha:colorA];
        conf.navigationBarHeight = barHeight;
        conf.goBackImagePathForFullScreenNavigation = backButtonImageResource;
        conf.closeImagePathForFullScreenNavigation = closeButtonImageResource;
        conf.isBackButtonVisible = buttonVisible;
        conf.contentMode = contentMode;
    }
    
    [TCGBWebView showWebViewWithURL:url viewController:[[TCGBViewControllerManager sharedGamebaseViewControllerManager] getViewController] configuration:conf closeCompletion:^(TCGBError *error){
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:message.scheme handle:message.handle TCGBError:error jsonData:nil extraData:[NSString stringWithFormat:@"%d", schemeEvent]];
        pluginData.completion(pluginData.jsonData, responseMessage);
    } schemeList:schemeArray schemeEvent:^(NSString *fullUrl, TCGBError *error){
        NativeMessage* responseMessage = [[NativeMessage alloc]initWithMessage:WEBVIEW_API_SCHEME_EVENT handle:schemeEvent TCGBError:error jsonData:fullUrl extraData:nil];
        pluginData.completion(pluginData.jsonData, responseMessage);
    }];
}

-(void)closeWebview:(TCGBPluginData*)pluginData {
    [TCGBWebView closeWebView];
}
@end

