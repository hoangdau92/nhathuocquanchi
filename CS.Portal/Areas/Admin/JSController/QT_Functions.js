jQuery(function () {
    jQuery("#listController").change(function () {
        var controllerName = jQuery(this).val();
        AJAXGetActionInController(controllerName);
    });
    mergeColumn('tbl-functions');
});

function clearSearch() {
    jQuery('#txtSearch').val("");
    jQuery("#btnTimKiem").click();
}

function del(id) {
    var obj = jQuery('#td_' + id).html();
    jQuery.confirm({
        title: 'Xóa chức năng',
        content: 'Bạn muốn xóa "<b>' + obj + '</b>"?',
        buttons: {
            confirm: {
                text: 'Đồng ý',
                action: function () {
                    var pJson = { 'id': id };
                    jQuery.ajax({
                        url: SiteUrlAdmin + "/QT_Functions/Delete",
                        contentType: 'application/json',
                        data: pJson,
                        dataType: 'json',
                        type: "GET",
                        success: function (response) {
                            if (response.status == true) {
                                jQuery("#btnTimKiem").click();
                            }
                            else {
                                alertError(response.message);
                            }
                        }
                    });
                }
            },
            cancel: {
                text: 'Hủy',
                action: function () {

                }
            }
        }
    });
}

function AJAXGetActionInController(controller) {
    try {
        var pJson = { 'controllerName': controller };
        jQuery.ajax({
            url: SiteUrlAdmin + "/QT_Functions/GetActionInController",
            contentType: 'application/json',
            dataType: 'json',
            data: pJson,
            type: "GET",
            success: function (response) {
                jQuery("#listAction").removeAttr("disabled");
                var htmlContent = '';
                if (response.data != null && response.data.length > 0) {
                    for (var i = 0; i < response.data.length; i++) {
                        htmlContent += '<option value="' + response.data[i] + '">' + response.data[i] + '</option>';
                    }
                }
                if (htmlContent != '') {
                    jQuery('#listAction').html(htmlContent);
                }
                else {
                    htmlContent = '<option>Chọn Action</option>';
                    jQuery("#listAction").attr('disabled', 'disabled');
                    jQuery('#listAction').html(htmlContent);
                }
            }
        });
    } catch (e) {
        alertError(e);
        return false;
    }
}

function submitEdit() {
    try {
        var pJson = {
            'ID': jQuery("#ID").val(),
            'Name': jQuery("#Name").val(),
            'Description': jQuery("#Description").val(),
            'ParentID': jQuery("#ParentID").val(),
            'ModuleID': jQuery("#ModuleID").val(),
            'Controller_Action': jQuery("#listController").val() + '-' + jQuery("#listAction").val()
        };
        console.log(pJson);
        jQuery.ajax({
            url: SiteUrlAdmin + "/QT_Functions/EditSubmit",
            contentType: 'application/json',
            dataType: 'json',
            data: pJson,
            type: "GET",
            success: function (response) {
                if (response.status == true) {
                    window.location.replace(SiteUrlAdmin + "/QT_Functions");
                }
                else {
                    alertError(response.message);
                }
            }
        });
    } catch (e) {
        alertError(e);
        return false;
    }
}
