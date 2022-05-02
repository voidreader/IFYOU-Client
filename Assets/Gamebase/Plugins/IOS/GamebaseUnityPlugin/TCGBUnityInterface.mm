#import "TCGBUnityInterface.h"
#import <Gamebase/Gamebase.h>
#import <GamebasePlugin/GamebasePlugin.h>

@implementation TCGBUnityInterface

static bool _startUnityScheduled = false;

+ (TCGBUnityInterface *)sharedUnityInterface {
    static dispatch_once_t onceToken;
    static TCGBUnityInterface* instance;
    dispatch_once(&onceToken, ^{
        instance = [[TCGBUnityInterface alloc] init];
    });
    return instance;
}

+ (void)load {
    UnityRegisterAppDelegateListener([TCGBUnityInterface sharedUnityInterface]);
}

- (void)setupViewController {
    [TCGBUtil logDebugWithFormat:@"[TCGB][Plugin][TCGBUnityInterface] setupViewController"];
    [[TCGBViewControllerManager sharedGamebaseViewControllerManager] setViewController:UnityGetGLViewController()];
}

#pragma mark - LifeCycleListener
- (void)didFinishLaunching:(NSNotification*)notification {
    [TCGBGamebase application:[UIApplication sharedApplication] didFinishLaunchingWithOptions:[notification userInfo]];
}

- (void)didBecomeActive:(NSNotification*)notification {
    if (!_startUnityScheduled)
    {
        _startUnityScheduled = true;
        [self performSelector: @selector(setupViewController) withObject: nil afterDelay: 0];
    }
    
    if ([TCGBGamebase appID] != nil) {
        [TCGBGamebase applicationDidBecomeActive:[UIApplication sharedApplication]];
    }
}

- (void)willResignActive:(NSNotification*)notification {
    if ([TCGBGamebase appID] != nil) {
        [TCGBGamebase applicationWillResignActive:[UIApplication sharedApplication]];
    }
}

- (void)didEnterBackground:(NSNotification*)notification {
    if ([TCGBGamebase appID] != nil) {
        [TCGBGamebase applicationDidEnterBackground:[UIApplication sharedApplication]];
    }
}

- (void)willEnterForeground:(NSNotification*)notification {
    if ([TCGBGamebase appID] != nil) {
        [TCGBGamebase applicationWillEnterForeground:[UIApplication sharedApplication]];
    }
}

- (void)willTerminate:(NSNotification*)notification {
    if ([TCGBGamebase appID] != nil) {
        [TCGBGamebase applicationWillTerminate:[UIApplication sharedApplication]];
    }
}

#pragma mark - AppDelegateListener
- (void)onOpenURL:(NSNotification*)notification {
    NSURL* url = [notification userInfo][@"url"];
    [TCGBGamebase application:[UIApplication sharedApplication] openURL:url sourceApplication:[notification userInfo][@"sourceApplication"] annotation:[notification userInfo][@"annotation"]];
}

// these are just hooks to existing notifications
- (void)applicationDidReceiveMemoryWarning:(NSNotification*)notification {
    [TCGBGamebase applicationDidReceiveMemoryWarning:[UIApplication sharedApplication]];
}

- (void)applicationSignificantTimeChange:(NSNotification*)notification {
    [TCGBGamebase applicationSignificantTimeChange:[UIApplication sharedApplication]];
}
@end
