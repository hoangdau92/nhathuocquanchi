function del(id) {
    var obj = jQuery('#td_' + id).html();
    jQuery.confirm({
        title: 'Xóa nhóm người dùng',
        content: 'Bạn muốn xóa "<b>' + obj + '</b>"?',
        buttons: {
            confirm: {
                text: 'Đồng ý',
                action: function () {
                    var pJson = { 'id': id };
                    jQuery.ajax({
                        url: SiteUrlAdmin + "/QT_Roles/Delete",
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

function clearSearch() {
    jQuery('#txtSearch').val("");
    jQuery("#btnTimKiem").click();
}