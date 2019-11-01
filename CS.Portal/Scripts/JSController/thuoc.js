var QUANCHI_Thuoc = {
    Loc: function () {
        var cate = $('#_CATE').val();
        var spmoi = $('#spmoi').val();
        var spuudai = $('#spuudai').val();
        var spdocquyen = $('#spdocquyen').val();
        var pJson = { cate: cate, spmoi: spmoi, spuudai: spuudai, spdocquyen: spdocquyen };
        jQuery.ajax({
            url: SiteUrl + "/Thuoc/Loc",
            contentType: 'application/json',
            data: pJson,
            dataType: 'json',
            type: "GET",
            success: function (response) {
                if (response.status == true) {
                    QUANCHI_Thuoc.BindData(response.data)
                }
                else {
                    alertError(response.message);
                }
            }
        });
    },

    BindData: function (data) {
        let content = ``;
        if (data.length > 0) {
            for (var i = 0; i < data.length; i++) {
                content += `<div class="item">`;
                content += `<a href="${SiteUrl}/chi-tiet-san-pham/${data[i].ten}-${data[i].id}">`;
                content += `<img class="img_${data[i].id}" src="${data[i].anhdaidien}" alt="hinh dai dien" />`;
                content += `</a>`;

                content += `<div class="item-info"><h3>`;
                content += `<a href="${SiteUrl}/chi-tiet-san-pham/${data[i].ten}-${data[i].id}" class="sanpham_${data[i].id}">${data[i].ten}</a>`;
                content += `</h3><p>${numberToMoney(data[i].gia)} đ</p></div>`;

                content += `<div class="item-button">`;
                content += `<a href="javascript:void(0)" onclick="QUANCHI_DonHang.AddtoCart(${data[i].id})" title="Thêm vào giỏ hàng" class="addtocart">`;
                content += `<i class="fas fa-shopping-basket"></i>`;
                content += `</a><a href="javascript:void(0)" class="seemore" title="Xem các sản phẩm cùng loại">`;
                content += `<i class="fas fa-align-center"></i>`;
                content += `</a></div></div>`;

            }
        }
        else {
        }
        $("#sanpham").html(content);
    },
}

jQuery(function () {
    jQuery('input[type="checkbox"]').on('click', function () {
        jQuery(this).val(this.checked ? true : false);
        QUANCHI_Thuoc.Loc();
    });
});