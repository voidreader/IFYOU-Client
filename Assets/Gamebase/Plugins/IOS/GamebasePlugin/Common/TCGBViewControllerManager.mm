#import <Foundation/Foundation.h>
#import "TCGBViewControllerManager.h"
#import <objc/runtime.h>

@implementation TCGBViewControllerManager

@synthesize viewController                       = _viewController;

+(TCGBViewControllerManager*)sharedGamebaseViewControllerManager {
    static dispatch_once_t onceToken;
    static TCGBViewControllerManager* instance = nil;
    dispatch_once(&onceToken, ^{
        instance = [[TCGBViewControllerManager alloc] init];
    });
    return instance;
}

-(UIViewController*)getViewController {
	return _viewController;
}

-(void)setViewController:(UIViewController*)viewController {
	_viewController = viewController;
}

@end
