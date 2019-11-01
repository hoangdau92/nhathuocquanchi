var SiteUrl = "http://localhost/nhathuoc_quanchi";
var SiteUrlAdmin = "http://localhost/nhathuoc_quanchi/admin";
var SiteUrlImgCKFinder = "http://localhost";

//---------------------START-HOANGND-------------------------

function alertSuccess(mess) {
    jQuery('#AlertBoxJS').removeClass().html(mess);
    jQuery('#AlertBoxJS').addClass("alert alert-success");
    jQuery('#AlertBoxJS').show(500);
    jQuery('#AlertBoxJS').delay(2000).hide(1000);
}

function alertError(mess) {
    jQuery('#AlertBoxJS').removeClass().html(mess);
    jQuery('#AlertBoxJS').addClass("alert alert-danger");
    jQuery('#AlertBoxJS').show(500);
    jQuery('#AlertBoxJS').delay(2000).hide(1000);
}
function alertWarning(mess) {
    jQuery('#AlertBoxJS').removeClass().html(mess);
    jQuery('#AlertBoxJS').addClass("alert alert-warning");
    jQuery('#AlertBoxJS').show(500);
    jQuery('#AlertBoxJS').delay(2000).hide(1000);
}


//hover dropdown menu
jQuery(document).ready(function () {
    //var tDelay = 100;
    //jQuery('ul.nav li.dropdown').hover(function () {
    //    jQuery(this).find('.dropdown-menu').eq(0).stop(true, true).delay(tDelay).fadeIn(500);
    //}, function () {
    //    jQuery(this).find('.dropdown-menu').stop(true, true).delay(tDelay).fadeOut(500);
    //});
    //jQuery('ul.dropdown-menu li.dropdown-submenu').hover(function () {
    //    jQuery(this).find('.dropdown-menu').eq(0).stop(true, true).delay(tDelay).fadeIn(500);
    //}, function () {
    //    jQuery(this).find('.dropdown-menu').stop(true, true).delay(tDelay).fadeOut(500);
    //});
    //roleMenu();
    replaceUrl();
    $(".tarea-special").each(function () {
        adjustHeight(this);
    });

    //$("a[href='#top']").click(function () {
    //    $("html, body").animate({ scrollTop: 0 }, "slow");
    //    return false;
    //});
});

function numberToMoney(num) {
    return num.toFixed(1).replace(/\d(?=(\d{3})+\.)/g, '$&,').replace('.0', '');
}


function replaceUrl() {
    jQuery("img").each(function (index) {
        var src_img = jQuery(this).attr("src");
        src_img = src_img.replace('/nhathuoc_quanchi/nhathuoc_quanchi', '/nhathuoc_quanchi');
        jQuery(this).attr("src", src_img);
    });
}

function adjustHeight(el) {
    el.style.height = (el.scrollHeight > el.clientHeight) ? (el.scrollHeight) + "px" : "60px";
}

function jsonUnixToDateTime(jsonDate) {
    if (jsonDate !== undefined && jsonDate != null) {
        var date = new Date(parseInt(jsonDate.substr(6)));
        var dat = date.getDate() + "/" + (parseInt(date.getMonth()) + 1) + "/" + +date.getFullYear();
        return dat;
    }
    else
        return "";
}
//--------------------END-HOANGND----------------------------