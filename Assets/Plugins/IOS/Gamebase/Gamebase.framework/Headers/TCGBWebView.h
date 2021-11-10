//
//  TCGBWebView.h
//  TCGBWebKit
//
//  Created by NHN on 2016. 12. 20..
//  © NHN. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <WebKit/WebKit.h>
#import "TCGBWebViewConfiguration.h"

NS_ASSUME_NONNULL_BEGIN

@class TCGBError;
@class TCGBWebViewController;
@protocol TCGBWebViewDelegate;

typedef void(^TCGBWebViewCloseCompletion)(TCGBError * _Nullable error);
typedef void(^TCGBWebViewSchemeEvent)(NSString * _Nullable fullUrl, TCGBError * _Nullable error);

extern NSString * const kTCGBWebKitDomain;
extern NSString * const kTCGBWebKitBundleName;

/** The TCGBWebView class represents the entry point for **launching WebView**.
 */
@interface TCGBWebView : NSObject
/**---------------------------------------------------------------------------------------
 * @name Properties
 *  ---------------------------------------------------------------------------------------
 */

/**
 
 This property is a global configuration for launching webview.<br/>
 When you handle the webview without any configuration, TCGBWebView set its configuration with this value.
 */
@property (nonatomic, strong) TCGBWebViewConfiguration *defaultWebConfiguration;


/**---------------------------------------------------------------------------------------
 * @name Initialization
 *  ---------------------------------------------------------------------------------------
 */

/**
 
 Creates and returns an `TCGBWebView` object.
 */
+ (instancetype)sharedTCGBWebView;

/**---------------------------------------------------------------------------------------
 * @name Launching WebView
 *  ---------------------------------------------------------------------------------------
 */

/**
 Show WebView that is not for local url.
 
 @param urlString The string value for target url
 @param viewController The presenting view controller
 @warning If viewController is nil, TCGBWebView set it to top most view controller automatically.
 @param configuration This configuration is applied to the behavior of webview.
 @warning If configuration is nil, TCGBWebView set it to default value. It is described in `TCGBWebViewConfiguration`.
 @param closeCompletion This completion would be called when webview is closed
 @param schemeList This schemeList would be filtered every web view request and call schemeEvent
 @param schemeEvent This schemeEvent would be called when web view request matches one of the schemeLlist
 
 @since Added 1.5.0.
 */
+ (void)showWebViewWithURL:(NSString *)urlString
            viewController:(nullable UIViewController *)viewController
             configuration:(nullable TCGBWebViewConfiguration *)configuration
           closeCompletion:(nullable TCGBWebViewCloseCompletion)closeCompletion
                schemeList:(nullable NSArray<NSString *> *)schemeList
               schemeEvent:(nullable TCGBWebViewSchemeEvent)schemeEvent;


/**
 Show WebView for local html (or other web resources)
 
 @param filePath The string value for target local path.
 @param bundle where the html file is located.
 @warning If bundle is nil, TCGBWebView set it to main bundle automatically.
 @param viewController The presenting view controller
 @warning If viewController is nil, TCGBWebView set it to top most view controller automatically.
 @param configuration This configuration is applied to the behavior of webview.
 @warning If configuration is nil, TCGBWebView set it to default value. It is described in `TCGBWebViewConfiguration`.
 @param closeCompletion This completion would be called when webview is closed
 @param schemeList This schemeList would be filtered every web view request and call schemeEvent
 @param schemeEvent This schemeEvent would be called when web view request matches one of the schemeLlist
 
 @since Added 1.5.0.
 */
+ (void)showWebViewWithLocalURL:(NSString *)filePath
                         bundle:(nullable NSBundle *)bundle
                 viewController:(nullable UIViewController *)viewController
                  configuration:(nullable TCGBWebViewConfiguration *)configuration
                closeCompletion:(nullable TCGBWebViewCloseCompletion)closeCompletion
                     schemeList:(nullable NSArray<NSString *> *)schemeList
                    schemeEvent:(nullable TCGBWebViewSchemeEvent)schemeEvent;

+ (void)showWebViewWithDefaultHTML:(NSString *)defaultHTML viewController:(nullable UIViewController *)viewController configuration:(nullable TCGBWebViewConfiguration *)configuration closeCompletion:(nullable TCGBWebViewCloseCompletion)closeCompletion schemeList:(nullable NSArray<NSString *> *)schemeList schemeEvent:(nullable TCGBWebViewSchemeEvent)schemeEvent;


/**
 Open the Browser with urlString
 
 @param urlString The URL to be loaded.
 @warning If urlString is not valid, to open browser would be failed. Please check the url before calling.
 
 @since Added 1.5.0.
 */
+ (void)openWebBrowserWithURL:(NSString *)urlString;


/**
 Close the presenting Webview
 
 @since Added 1.5.0.
 */
+ (void)closeWebView;

@end


/** The TCGBWebViewDelegate is a UIViewController delegate.
 */
@protocol TCGBWebViewDelegate <NSObject>

@required

@optional
- (void)viewDidAppear:(BOOL)animated;
- (void)viewDidDisappear:(BOOL)animated;
- (void)close;
- (void)goBack;
- (void)click;
- (void)neverShowToday;

//- (void)webViewDidStartLoad:(UIWebView *)webView;
- (void)webView:(WKWebView *)webView didCommitNavigation:(null_unspecified WKNavigation *)navigation;

//- (void)webViewDidFinishLoad:(UIWebView *)webView;
- (void)webView:(WKWebView *)webView didFinishNavigation:(null_unspecified WKNavigation *)navigation;

//- (BOOL)webView:(UIWebView *)webView shouldStartLoadWithRequest:(NSURLRequest *)request navigationType:(UIWebViewNavigationType)navigationType;
- (void)webView:(WKWebView *)webView decidePolicyForNavigationAction:(WKNavigationAction *)navigationAction decisionHandler:(void (^)(WKNavigationActionPolicy))decisionHandler;

- (void)webView:(WKWebView *)webView didFailProvisionalNavigation:(null_unspecified WKNavigation *)navigation withError:(NSError *)error;

//- (void)webView:(UIWebView *)webView didFailLoadWithError:(NSError *)error;
- (void)webView:(WKWebView *)webView didFailNavigation:(null_unspecified WKNavigation *)navigation withError:(NSError *)error;

//- (NSString *)stringByEvaluatingJavaScriptFromString:(NSString *)script;
- (void)evaluateJavaScript:(NSString *)script completionHandler:(void(^ _Nullable)(_Nullable id data, NSError * _Nullable error))completion;

@property (nonatomic, weak) UIView *rootView;

@end

NS_ASSUME_NONNULL_END
