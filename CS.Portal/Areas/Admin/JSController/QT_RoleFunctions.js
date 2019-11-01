jQuery(function () {
    mergeColumn('tbl-quyen');
    AJAXGetDataPermission();
    jQuery("#lmodule").change(function () {
        if (ROLEID != 0) {
            selectRole(ROLEID);
        }
    });
    jQuery("#lpage").change(function () {
        if (ROLEID != 0) {
            selectRole(ROLEID);
        }
    });
});

function AJAXGetDataPermission() {
    jQuery.ajax({
        url: SiteUrlAdmin + "/QT_RoleFunction/GetDataPermission",
        contentType: 'application/json',
        dataType: 'json',
        type: "POST",
        success: function (r) {
            if (r.state == true) {
                bindDataRole(r.lRole);
                bindListModule(r.lModule);
            }
        }
    });
}

function bindListModule(data) {
    var content = '';
    for (var i = 0; i < data.length; i++) {
        content += '<option value="' + data[i].ID + '">' + data[i].Name + '</option>';
    }
    jQuery('#lmodule').html(content);
}

function bindDataRole(data) {
    var content = '';
    for (var i = 0; i < data.length; i++) {
        content += '<tr name="' + data[i].ID + '" onclick="selectRole(' + data[i].ID + ')" class="select-row">';
        content += '<td>' + data[i].Name + '</td>';
        content += '</tr>';
    }
    jQuery('#tbl-role').html(content);
}

function bindDataPage(data) {
    var content = '';
    var moduleid = 0;
    for (var i = 0; i < data.length; i++) {
        if (data[i].ModuleID != moduleid) {
            content += '<h5 style="font-style: italic; text-align: center;">---- ' + data[i].ModuleName + ' ----</h5>';
            moduleid = data[i].ModuleID;
        }
        content += '<li class="list-group-item" id="page_' + data[i].ID + '" name="' + data[i].ID + '">' + data[i].Name + '</li>';
    }
    jQuery('#ul-page').html(content);
}

var ROLEID = 0;
function selectRole(RoleID) {
    ROLEID = RoleID;
    jQuery('#tbl-role tr.bg-primary').removeClass('bg-primary');
    jQuery('#tbl-role tr[name="' + RoleID + '"]').addClass('bg-primary');
    jQuery('#RoleID').val(RoleID);
    var moduleid = jQuery('#lmodule').val();
    var isadmin = jQuery('#lpage').val();
    var pJson = { 'RoleID': RoleID, 'ModuleID': moduleid, 'isadmin': isadmin };
    jQuery.ajax({
        url: SiteUrlAdmin + "/QT_RoleFunction/GetPermissionByRole",
        contentType: 'application/json',
        dataType: 'json',
        data: JSON.stringify(pJson),
        type: "POST",
        success: function (r) {
            if (r.state == true) {
                //////build tree
                jQuery('#jsTreeContentP').html('<div id="jsTreeContent"></div>');
                jQuery('#jsTreeContent').jstree({
                    'core': {
                        'data': r.jsTreeList
                    },
                    "checkbox": {
                        "keep_selected_style": false,
                        "three_state": false,
                        "cascade": "undetermined"
                    },
                    "plugins": ["checkbox"]
                });
                //////build tree function
                jQuery('#jsTreeFuncP').html('<div id="jsTreeFunc"></div>');
                jQuery('#jsTreeFunc').jstree({
                    'core': {
                        'data': r.jsTreeListF
                    },
                    "checkbox": {
                        "keep_selected_style": false,
                        "three_state": false,
                        "cascade": "undetermined"
                    },
                    "plugins": ["checkbox"]
                });
            }
        }
    });
}

function saveFunc() {
    var RoleID = jQuery('#RoleID').val();
    var ModuleID = jQuery('#lmodule').val();
    var selectedElmsIds = jQuery('#jsTreeFunc').jstree("get_selected");
    jQuery("#jsTreeFunc").find(".jstree-undetermined").each(function (i, element) {
        selectedElmsIds.push(jQuery(element).closest('.jstree-node').attr("id"));
    });
    if (RoleID == 0) {
        alertWarning('Chưa chọn nhóm người dùng!');
    }
    else {
        var pJson = { 'RoleID': RoleID, 'ModuleID': ModuleID, 'FuncIDs': selectedElmsIds.toString() };
        jQuery.ajax({
            url: SiteUrlAdmin + "/QT_RoleFunction/SaveRoleFunction",
            contentType: 'application/json',
            dataType: 'json',
            data: JSON.stringify(pJson),
            type: "POST",
            success: function (r) {
                if (r.state == true) {
                    alertSuccess(r.message);
                }
                else {
                    alertError(r.message);
                }
            }
        });
    }
}

function savePage() {
    var isadmin = jQuery('#lpage').val();
    var selectedElmsIds = jQuery('#jsTreeContent').jstree("get_selected");
    jQuery("#jsTreeContent").find(".jstree-undetermined").each(function (i, element) {
        selectedElmsIds.push(jQuery(element).closest('.jstree-node').attr("id"));
    });
    var RoleID = jQuery('#RoleID').val();
    if (RoleID == 0) {
        alertWarning('Chưa chọn nhóm người dùng!');
    }
    else {
        //PageIDs = PageIDs.slice(0, -1);
        var pJson = { 'RoleID': RoleID, 'PageIDs': selectedElmsIds.toString(), 'isadmin': isadmin };
        jQuery.ajax({
            url: SiteUrlAdmin + "/QT_RoleFunction/SavePageRole",
            contentType: 'application/json',
            dataType: 'json',
            data: JSON.stringify(pJson),
            type: "POST",
            success: function (r) {
                if (r.state == true) {
                    alertSuccess(r.message);
                }
                else {
                    alertError(r.message);
                }
            }
        });
    }
}









































function clearSearch() {
    jQuery('#txtSearch').val("");
    jQuery("#btnTimKiem").click();
}

function deleteRF(id) {
    var obj = jQuery('#td_' + id).html();
    jQuery.confirm({
        title: 'Xóa chức năng',
        content: 'Bạn muốn xóa "<b>Nhóm người dùng - chức năng</b> này"?',
        buttons: {
            confirm: {
                text: 'Đồng ý',
                action: function () {
                    var pJson = { 'id': id };
                    jQuery.ajax({
                        url: SiteUrlAdmin + "/Admin/QT_RoleFunction/Delete",
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

function mergeColumn(idtable) {
    var flag = 0;
    jQuery('#' + idtable + ' > tbody  > tr').each(function () {
        var _class = jQuery(this).attr('class');
        var td = jQuery(this).find('td[name=mergeColumn]').eq(0);
        var id = _class.split('_')[1];
        if (id != flag) {
            td.attr('rowspan', jQuery('.' + _class).length).css('vertical-align', 'middle');
            flag = id;
        }
        else {
            td.remove();
        }
    });
}