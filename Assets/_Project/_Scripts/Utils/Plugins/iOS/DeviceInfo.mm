
#import <UIKit/UIKit.h>

extern "C"
{
    float _screenScaleFactor()
    {
        return [[UIScreen mainScreen] scale];
    }
    
    bool _isTablet()
    {
        return UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad;
    }
    
    void _openAppSettings()
    {
        [[UIApplication sharedApplication] openURL:[NSURL URLWithString:UIApplicationOpenSettingsURLString]];
    }
}
