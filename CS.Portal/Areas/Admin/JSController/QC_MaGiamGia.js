jQuery(function () {
    $("#idloaigiamgia").change(function () {
        if ($(this).val() == 'TIEN') {
            $("#giatri-addon").text("VNĐ");
        }
        else {
            $("#giatri-addon").text("%");
        }
    });

    jQuery('#tungay').datepicker({
        dateFormat: 'dd-mm-yy'
    });

    jQuery('#denngay').datepicker({
        dateFormat: 'dd-mm-yy'
    });

    $("#idkieudoituong").change(function () {
        if ($(this).val() == '') {
            $("#chondoituong").addClass("hidden");
        }
        else {
            LayDoiTuong(false);
        }
    });

    jQuery('input[type="checkbox"]').on('click', function () {
        jQuery(this).val(this.checked ? true : false);
    });
});

function LayDoiTuong(getSelected) {
    let kieudoituong = $("#idkieudoituong").val();
    var pJson = { 'kieudoituong': kieudoituong };
    jQuery.ajax({
        url: SiteUrlAdmin + "/QC_MaGiamGia/LayDoiTuong",
        contentType: 'application/json',
        data: pJson,
        dataType: 'json',
        type: "GET",
        success: function (response) {
            if (response.status == true) {
                $("#chondoituong").removeClass("hidden");
                let data = response.result;
                let content = `<option value="-1">---- Tất cả ----</option>`;
                for (var i = 0; i < data.length; i++) {
                    content += `<option value="${data[i].id}">${data[i].tenhienthi}</option>`;
                }
                $('#doituong').html(content);
                $('#doituong').select2();
                if (getSelected) {
                    $("#doituong").val($("#lDoiTuong").val().split(','));
                    $('#doituong').select2().trigger('change');
                    $("#doituong").prop("disabled", 1);
                }
            }
            else {
                alertError(response.message);
            }
        }
    });
}