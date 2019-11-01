jQuery(function () {
    jQuery('input[type="checkbox"]').on('click', function () {
        jQuery(this).val(this.checked ? true : false);
    });
    jQuery("form").submit(function (e) {
        e.preventDefault();
        AJAXSaveData();
    });
    AJAXGetDataPageSetup();
});

function AJAXGetDataPageSetup() {
    var pageid = jQuery('#pageid').val();
    var pJson = { 'pageid': pageid };
    jQuery.ajax({
        url: SiteUrlAdmin + "/QT_Pages/GetDataPageSetup",
        contentType: 'application/json',
        dataType: 'json',
        data: JSON.stringify(pJson),
        type: "POST",
        success: function (r) {
            if (r.state == true) {
                //BindDataPageSetup(r.lData);
                BindDataPageSetup(r.listPPB);
                var data = r.modules;
                var content = '<option value="">--  Chọn module  --</option>';
                for (var i = 0; i < data.length; i++) {
                    content += '<option value="' + data[i].ID + '">' + data[i].Name + '</option>';
                }
                jQuery('#ModuleID').html(content);
                //dataBox
                var dataBox = r.lDataBox;
                var content = '<option value="">--  Chọn box  --</option>';
                for (var i = 0; i < dataBox.length; i++) {
                    content += '<option value="' + dataBox[i].ID + '">' + dataBox[i].Title + '</option>';
                }
                jQuery('#BoxParent').html(content);
                //bind data list page to copy
                var dataPage = r.listPage;
                var content = '<option value="">--  Chọn trang  --</option>';
                for (var i = 0; i < dataPage.length; i++) {
                    content += '<option value="' + dataPage[i].ID + '">' + dataPage[i].Name + '</option>';
                }
                jQuery('#sltPage').html(content);
            }
        }
    });
}

function BindDataPageSetup(data) {
    var content = '';
    for (var i = 0; i < data.length; i++) {
        content += '<tr style="background: #f2f2f2;"><td></td>';
        content += '<td align="center" name="mergeColumn">' + data[i].box.Row + '</td>';
        content += '<td><b>' + data[i].box.Title + '</b></td>';
        content += '<td></td>';
        content += '<td>' + data[i].box.CA + '</td>';
        content += '<td align="center">' + data[i].box.Position + '</td>';
        content += '<td align="center">' + data[i].box.Order + '</td>';
        content += '<td align="center"><input type="checkbox" disabled ' + (data[i].box.IsActive == true ? 'checked' : '') + '/></td>';
        content += '<td align="center"><button class="btn btn-default btn-xs" title="Sửa" onclick="editPage(' + data[i].box.ID + ')"><i class="far fa-edit"></i></button> ';
        content += '<button class="btn btn-danger btn-xs" title="Xóa" onclick="deleteP(' + data[i].box.ID + ')"><i class="far fa-trash-alt"></i></button></td>';
        var dataC = data[i].boxChild;
        for (var j = 0; j < dataC.length; j++) {
            content += '<tr class="tr_' + dataC[j].Row + '">';
            content += '<td align="center">' + (j + 1) + '</td>';
            content += '<td align="center" name="mergeColumn">' + dataC[j].Row + '</td>';
            content += '<td>' + dataC[j].Title + '</td>';
            content += '<td>' + dataC[j].PartialName + '</td>';
            content += '<td>' + dataC[j].CA + '</td>';
            content += '<td align="center">' + dataC[j].Position + '</td>';
            content += '<td align="center">' + dataC[j].Order + '</td>';
            content += '<td align="center"><input type="checkbox" disabled ' + (dataC[j].IsActive == true ? 'checked' : '') + '/></td>';
            content += '<td align="center"><button class="btn btn-default btn-xs" title="Sửa" onclick="editPage(' + dataC[j].ID + ')"><i class="far fa-edit"></i></button> ';
            content += '<button class="btn btn-danger btn-xs" title="Xóa" onclick="deleteP(' + dataC[j].ID + ')"><i class="far fa-trash-alt"></i></button></td>';
            content += '</tr>';
        }
    }
    jQuery('#tbl-pagesetup tbody').html(content);
    //mergeColumn('tbl-pagesetup');
}

function getPartials() {
    var moduleid = jQuery('#ModuleID').val();
    moduleid = moduleid == '' ? 0 : moduleid;
    var pJson = { 'moduleid': moduleid };
    jQuery.ajax({
        url: SiteUrlAdmin + "/QT_Pages/getPartials",
        contentType: 'application/json',
        dataType: 'json',
        data: JSON.stringify(pJson),
        type: "POST",
        success: function (r) {
            if (r.state == true) {
                var data = r.partials;
                var content = '<option value="">--  Chọn partial  --</option>';
                for (var i = 0; i < data.length; i++) {
                    content += '<option value="' + data[i].ID + '">' + data[i].Name + '</option>';
                }
                jQuery('#PartialID').html(content);
            }
        }
    });
}

function AJAXSaveData() {
    var ID = jQuery('#ID').val();
    var PageID = jQuery('#pageid').val();
    var Title = jQuery('#Title').val();
    var PartialID = jQuery('#PartialID').val();
    var arrPosition = jQuery('#Position').val();
    var Row = jQuery('#Row').val();
    var Order = jQuery('#Order').val();
    var IsActive = jQuery('#IsActive').val();
    var Position = arrPosition.toString().replace(/,/g, " ");
    var IsBox = jQuery('#IsBox').val();
    var BoxParent = jQuery('#BoxParent').val();
    var obj = {
        ID: ID, Title: Title, PartialID: PartialID, Position: Position, Row: Row
        , Order: Order, IsActive: IsActive, PageID: PageID
        , IsBox: IsBox, BoxParent: BoxParent
    };
    var pJson = { 'obj': obj };
    jQuery.ajax({
        url: SiteUrlAdmin + "/QT_Pages/SaveSetupPage",
        contentType: 'application/json',
        dataType: 'json',
        data: JSON.stringify(pJson),
        type: "POST",
        success: function (r) {
            if (r.state == true) {
                AJAXGetDataPageSetup();
                alertSuccess(r.message);
                cancel();
            }
            else {
                alertError(r.message);
            }
        }
    });
}

function editPage(id) {
    var pJson = { 'id': id };
    jQuery.ajax({
        url: SiteUrlAdmin + "/QT_Pages/getDetailPagePartial",
        contentType: 'application/json',
        dataType: 'json',
        data: JSON.stringify(pJson),
        type: "POST",
        success: function (r) {
            if (r.state == true) {
                var data = r.obj;
                var data1 = r.lPartials;
                var content = '<option value="">--  Chọn partial  --</option>';
                for (var i = 0; i < data1.length; i++) {
                    content += '<option value="' + data1[i].ID + '">' + data1[i].Name + '</option>';
                }
                jQuery('#PartialID').html(content);
                jQuery.each(data, function (key, value) {
                    var v = value != null ? value : "";
                    if (key == 'Position') {
                        var arr = v.split(' ');
                        jQuery('#' + key).val(arr);
                        jQuery('.selectpicker').selectpicker('refresh');
                    }
                    else
                        jQuery('#' + key).val(v);
                });
                if (data["IsActive"] == true) {
                    jQuery('#IsActive').prop("checked", true).val(true);
                }
                else {
                    jQuery('#IsActive').prop("checked", false).val(false);
                }
                if (data["IsBox"] == true) {
                    jQuery('#IsBox').prop("checked", true).val(true);
                }
                else {
                    jQuery('#IsBox').prop("checked", false).val(false);
                }
                jQuery('#btnSave').text('Cập nhật');
            }
        }
    });
}

function cancel() {
    jQuery('#ID').val(0);
    jQuery('#Title').val('');
    jQuery('#Row').val('');
    jQuery('#ModuleID').val('');
    jQuery('#PartialID').val('');
    jQuery('#Order').val('');
    jQuery('#Position').val('');
    jQuery('#ID').val(0);
    jQuery('.selectpicker').selectpicker('refresh');
    jQuery('#btnSave').text('Thêm mới');
}

function deleteP(id) {
    jQuery.confirm({
        title: '<i class="fa fa-trash" aria-hidden="true"></i> WARNING',
        content: '<i class="fa fa-arrow-right" aria-hidden="true"></i> Bạn muốn xóa partial khỏi page?',
        type: 'red',
        async: false,
        buttons: {
            confirm: {
                text: 'Đồng ý',
                btnClass: 'btn-red',
                action: function () {
                    var pJson = { 'id': id };
                    jQuery.ajax({
                        url: SiteUrlAdmin + "/QT_Pages/DeletePagePartial",
                        contentType: 'application/json',
                        data: pJson,
                        dataType: 'json',
                        type: "GET",
                        success: function (r) {
                            if (r.state == true) {
                                AJAXGetDataPageSetup();
                                alertSuccess(r.message);
                            }
                            else {
                                alertError(r.message);
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

function copypage() {
    var pagenguon = jQuery('#sltPage').val();
    var pagedich = jQuery('#pageid').val();
    if (pagedich != null && pagenguon != null) {
        var pJson = { 'pagenguon': pagenguon, 'pagedich': pagedich };
        jQuery.ajax({
            url: SiteUrlAdmin + "/QT_Pages/CopyPageSetup",
            contentType: 'application/json',
            dataType: 'json',
            data: JSON.stringify(pJson),
            type: "POST",
            success: function (r) {
                if (r.state == true) {
                    AJAXGetDataPageSetup();
                    alertSuccess(r.message);
                }
                else {
                    alertError(r.message);
                }
            }
        });
    }
}