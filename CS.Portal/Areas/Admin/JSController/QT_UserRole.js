jQuery(function () {
    //Sự kiện nút chọn tất cả
    jQuery('#checkAllNotInGroup:button').click(function () {
        var checked = !jQuery(this).data('checked');
        jQuery('input[name="chkUsersNotInGroup"]:checkbox').prop('checked', checked);
        jQuery(this).val(checked ? 'uncheck all' : 'check all')
        jQuery(this).data('checked', checked);
    });

    jQuery('#checkAllInGroup:button').click(function () {
        var checked = !jQuery(this).data('checked');
        jQuery('input[name="chkUsersInGroup"]:checkbox').prop('checked', checked);
        jQuery(this).val(checked ? 'uncheck all' : 'check all')
        jQuery(this).data('checked', checked);
    });

    //Sự kiện ddlRoles onchange
    jQuery("#ddlRoles").change(function () {
        var strRoleID = jQuery(this).val();
        jQuery('#ipRoleID').val(strRoleID);

        jQuery('#lbxUserRole').find('option').remove();
        jQuery('#lbxUsers').find('option').remove();

        AJAXGetUsersGroup(jQuery('#ipSearch').val(), strRoleID);
    });

});

function clearForm() {
    jQuery('#ipSearch').val("");
    searchForm();
}

function searchForm() {
    var name = jQuery('#ipSearch').val();
    var strRoleID = jQuery("#ddlRoles").val();
    AJAXGetUsersGroup(name, strRoleID);
}

function AJAXGetUsersGroup(name, strRoleID) {
    try {
        var pJson = { 'Name': name, 'RoleID': strRoleID };
        jQuery.ajax({
            url: SiteUrlAdmin + "/QT_UserRole/GetUsersGroup",
            contentType: 'application/json',
            dataType: 'json',
            data: pJson,
            type: "GET",
            success: function (response) {
                //console.log(response);
                var listUserNotInGroup = response.listUserNotInGroup;
                var listUserInGroup = response.listUserInGroup;
                //fill user not in group
                var options = '';
                jQuery.each(listUserNotInGroup, function (i, item) {
                    options += "<div class='checkbox userGroup'><label title ='" + item.UserName + "' class='CSFtooltip'><input id='chkUser_" + item.ID + "' value='" + item.ID + "' name='chkUsersNotInGroup' type='checkbox'>" + item.FullName + "</label></div>";
                });
                jQuery('#divUsersNotInGroup').html(options);

                //fill user in group
                options = '';
                jQuery.each(listUserInGroup, function (i, item) {
                    options += "<div class='checkbox userGroup'><label title ='" + item.UserName + "' class='CSFtooltip'><input id='chkUser_" + item.ID + "' value='" + item.ID + "' name='chkUsersInGroup' type='checkbox'>" + item.FullName + "</label></div>";
                });
                jQuery('#divUsersInGroup').html(options);
            }
        });
    } catch (e) {
        alertError(e);
        return false;
    }
}

function setUserInGroup() {
    try {
        var checkedValues = jQuery('input[name="chkUsersNotInGroup"]:checked').map(function () {
            return this.value;
        }).get();

        var RoleID = jQuery("#ddlRoles").val()
        if (checkedValues.length == 0 || RoleID == 0) {
            alertErrMessage("Bạn chưa chọn người dùng hoặc nhóm người dùng!");
            return false;
        }

        var pJson = { 'arrChecked': checkedValues, 'RoleID': RoleID };
        jQuery.ajax({
            url: SiteUrlAdmin + "/QT_UserRole/SetUsersInGroup",
            contentType: 'application/json',
            dataType: 'json',
            data: JSON.stringify(pJson),
            type: "POST",
            success: function (response) {
                if (response == true) {
                    alertSuccess("Gán người dùng thành công");
                    AJAXGetUsersGroup(jQuery('#ipSearch').val(), RoleID);
                }
                else {
                    alertError("Lỗi gán người dùng");
                }
            }
        });
    } catch (e) {
        alertError(e);
        return false;
    }
}

function setUserOutGroup() {
    try {
        var checkedValues = jQuery('input[name="chkUsersInGroup"]:checked').map(function () {
            return this.value;
        }).get();

        var RoleID = jQuery("#ddlRoles").val()
        if (checkedValues.length == 0 || RoleID == 0) {
            alertErrMessage("Bạn chưa chọn người dùng hoặc nhóm người dùng!");
            return false;
        }

        var pJson = { 'arrChecked': checkedValues, 'RoleID': RoleID };
        jQuery.ajax({
            url: SiteUrlAdmin + "/QT_UserRole/SetUsersOutGroup",
            contentType: 'application/json',
            dataType: 'json',
            data: JSON.stringify(pJson),
            type: "POST",
            success: function (response) {
                if (response == true) {
                    alertSuccess("Gỡ người dùng khỏi nhóm thành công");
                    AJAXGetUsersGroup(jQuery('#ipSearch').val(), RoleID);
                }
                else {
                    alertError("Lỗi gỡ người dùng");
                }
            }
        });
    } catch (e) {
        alertError(e);
        return false;
    }
}


