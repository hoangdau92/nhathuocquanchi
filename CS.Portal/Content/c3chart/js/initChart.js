var ChartJS = function () {

    var handleChart_tienhangthang = function () {
        let dulieu = $.merge(["Tổng đơn hàng"], $("#tienhangthang").val().split(','));
        var chart = c3.generate({
            bindto: '#chart_tienhangthang',
            data: {
                x: 'x', 
                columns: [
                    //['x', 'Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6', 'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'],
                    ['x', 'Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6', 'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'],
                    dulieu,
                ],
                type: 'bar'
            },
            axis: {
                x: {
                    type: 'categorized'
                },
                y: {
                    tick: {
                        format: d3.format(",")
                    },
                    label: {
                        text: 'Tổng tiền (VND)',
                        position: 'outer-middle'
                    }
                }
            },
            bar: {
                width: {
                    ratio: 0.3,
                },
            }
        });
    };

    var handleChart_loaisanpham = function () {
        let loaisp = $("#loaisp").val().split(',');
        let soluongsp = $("#soluongsp").val().split(',');
        let data = [];
        for (var i = 0; i < loaisp.length; i++) {
            let temp = [loaisp[i], soluongsp[i]];
            data.push(temp);
        }
        var chart = c3.generate({
            bindto: '#chart_loaisanpham',
            data: {
                columns: data,
                type: 'pie',
            },
            pie: {
                label: {
                    format: function (value, ratio, id) {
                        return d3.format('')(value) + " sp";
                    }
                }
            }
        });
    };

    var handleChart_sanpham = function () {
        let tensp = $.merge(["x"], $("#tensp").val().split(','));
        let soluongban = $.merge(["Số lượng bán"], $("#soluongban").val().split(','));
        var chart = c3.generate({
            bindto: '#chart_sanpham',
            data: {
                x: 'x',
                columns: [
                    tensp,
                    soluongban,
                ],
                type: 'bar'
            },
            axis: {
                x: {
                    type: 'categorized'
                },
                y: {
                    label: {
                        text: 'Số lượng bán',
                        position: 'outer-middle'
                    }
                }
            },
            bar: {
                width: {
                    ratio: 0.3,
                },
            }
        });
    };

    var handleChart_khachhang = function () {
        let tenkh = $("#tenkh").val().split(',');
        let tienmua = $("#tienmua").val().split(',');
        var datacol = [];
        for (var i = 0; i < tenkh.length; i++) {
            let o = [tenkh[i], tienmua[i]];
            datacol.push(o);
        }
        var chart = c3.generate({
            bindto: '#chart_khachhang',
            data: {
                columns: datacol,
                type: 'bar'
            },
            axis: {
                x: {
                    type: 'categorized'
                },
                y: {
                    tick: {
                        format: d3.format(",")
                    },
                    label: {
                        text: 'Tổng tiền (VND)',
                        position: 'outer-middle'
                    }
                }
            },
            bar: {
                space: 0.25
            }
        });
    };

    return {
        //main function to initiate the module
        init: function () {
            handleChart_tienhangthang();
            handleChart_loaisanpham();
            handleChart_sanpham();
            handleChart_khachhang();
        }
    };

}();

jQuery(document).ready(function () {
    ChartJS.init();
});

