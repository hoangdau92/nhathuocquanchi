
function clearSearch() {
    jQuery('#txtSearch').val("");
    jQuery("#btnTimKiem").click();
}

function del(id) {
    jQuery.confirm({
        title: '<i class="fa fa-trash" aria-hidden="true"></i> WARNING',
        content: '<i class="fa fa-arrow-right" aria-hidden="true"></i> Bạn muốn xóa tin này?',
        type: 'red',
        async: false,
        buttons: {
            confirm: {
                text: 'Đồng ý',
                btnClass: 'btn-red',
                action: function () {
                    var pJson = { 'id': id };
                    jQuery.ajax({
                        url: SiteUrlAdmin + "/CMS_News/Delete",
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

function guiduyet(id, title) {
    jQuery.confirm({
        title: '<i class="far fa-share-square"></i> Gửi duyệt',
        content: '<i class="fa fa-arrow-right" aria-hidden="true"></i> Gửi duyệt tin bài <b>' + title + '</b> ?',
        type: 'blue',
        async: false,
        buttons: {
            confirm: {
                text: 'Đồng ý',
                btnClass: 'btn-blue',
                action: function () {
                    var pJson = { 'newsid': id };
                    jQuery.ajax({
                        url: SiteUrlAdmin + "/CMS_News/GuiDuyet",
                        contentType: 'application/json',
                        data: pJson,
                        dataType: 'json',
                        type: "GET",
                        success: function (response) {
                            if (response.state == true) {
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

function removeFile() {
    jQuery('#pre_picture').attr('src', '');
    jQuery('#PICTURE').val('');
}


function setcomment(id) {
    try {
        var pJson = { 'id': id };
        jQuery.ajax({
            url: SiteUrlAdmin + "/CMS_News/Comment",
            contentType: 'application/json',
            dataType: 'json',
            data: JSON.stringify(pJson),
            type: "POST",
            success: function (r) {
                console.log(r);
                if (r.state == true) {
                    var data = r.lData;
                    var content = '';
                    for (var i = 0; i < data.length; i++) {
                        content += '<tr>';
                        content += '<td align="center">' + (i + 1) + '</td>';
                        content += '<td>' + data[i].Comment + '</td>';
                        content += '<td align="center">' + data[i].TrangThai + '</td>';
                        content += '<td>' + data[i].FullName + '</td>';
                        content += '</tr>';

                    }
                    jQuery('#danhsachykien').html(content);
                }
             
            }
        });
        jQuery('#popupcomment').modal();
    } catch (e) {
        alertError(e);
        return false;
    }
}