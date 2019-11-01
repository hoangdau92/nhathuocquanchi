
function setXoa() {
    try {
        var checkedValues = jQuery('input[name="chkXoa"]:checked').map(function () {
            return this.value;
        }).get();
        if (checkedValues.length == 0) {
            alertError("Bạn chưa chọn dữ liệu để xóa");
            return false;
        }
        var pJson = { 'arrChecked': checkedValues};
        jQuery.ajax({
            url: SiteUrlAdmin + "/CSF_Logs/SetXoa",
            contentType: 'application/json',
            dataType: 'json',
            data: JSON.stringify(pJson),
            type: "POST",
            success: function (r) {
                console.log(r);
                if (r.status == true) {
                    alertSuccess(r.message);
                    setTimeout(function () {         
                        jQuery("#btnTimKiem").click();
                    }, 2000);

                }
                else {
                    alertSuccess(r.message);
                }
            }
        });
    } catch (e) {
        alertError(e);
        return false;
    }
}

function clearSearch() {
    jQuery('#txtSearch').val("");
    jQuery("#btnTimKiem").click();
}