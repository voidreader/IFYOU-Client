#import <Foundation/Foundation.h>

@interface TCGBViewControllerManager : NSObject {
}

@property (nonatomic, strong) UIViewController *viewController;

+(TCGBViewControllerManager*)sharedGamebaseViewControllerManager;

-(UIViewController*)getViewController;
-(void)setViewController:(UIViewController*)viewController;

@end
