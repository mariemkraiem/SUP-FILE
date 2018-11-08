
function notify(type, message) {
    var html = '<div class="alert alert-' + type + '"><span>' + message + '</span></div>';
    jQuery('#notifications').append(html);
    var notification = jQuery('#notifications div').last();
    jQuery(notification).hover(function () {
        jQuery(this).stop();
        jQuery(this).fadeIn('fast');
    });
    jQuery(notification).mouseleave(function () {
        jQuery(this).fadeOut(5000, function () {
            jQuery(this).remove();
        })
    });
    jQuery(notification).fadeOut(5000, function () {
        jQuery(this).remove();
    }); jQuery(notification).on('click', function () { jQuery(this).remove(); })
} 