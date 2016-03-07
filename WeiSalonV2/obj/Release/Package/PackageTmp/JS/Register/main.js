$(function () {
    //滚动条
    try {
        $('.case-detai').eq(0).scrollbar();
    } catch (e) { }

    if ($(window).height() >= $('body').height()) {
        $('body').css('min-height', $(window).height() - 118 + 'px');
    }

    if (typeof Swiper !== 'undefined') {
        var focus = new Swiper('.swiper-container', {
            pagination: '.pagination',
            paginationClickable: true
        });
    }


    //案例切换
    var oCaseList = $('.case-list');
    oCaseList.find('a').mouseenter(function () {
        var iStop = $(this).position().left;
        oCaseList.find('.case-arrow').stop().animate({ 'left': iStop + 'px' }, 300);
    }).mouseleave(function () {
        var iStop = oCaseList.find('.current').position().left;
        oCaseList.find('.case-arrow').stop().animate({ 'left': iStop + 'px' }, 300);
    }).click(function () {
        var index = oCaseList.find('a').index($(this));
        $(this).addClass('current').siblings().removeClass('current');
        oCaseList.siblings('.case-item').eq(index).show().siblings('.case-item').hide();
        if (!$('.case-detai').eq(index).hasClass('jq-scroll')) {
            $('.case-detai').eq(index).scrollbar();
        }
    }).eq(0).mouseenter();

    //登陆处理
    var oFormLogin = $('.login-form');
    $('.login-link').click(function (e) {
        oFormLogin.fadeIn();
        e.stopPropagation();
    });
    $(document).click(function (e) {
        var _self = e.target;
        if (_self.className !== 'login-form' && $(_self).parents('.login-form').length === 0) {
            oFormLogin.fadeOut();
        }
    });

    $('.login-form .username').on({
        focus: function () {
            if ($(this).val() == '用户名') {
                $(this).val('');
            }
        },
        blur: function () {
            if ($(this).val() == '') {
                $(this).val('用户名');
            }
        }
    });
    $('.login-form .pwd-text').on('focus', function () {
        if ($(this).val() == '密码') {
            $(this).hide().siblings('.pwd-point').show().focus();
        }
    });
    $('.login-form .pwd-point').on('blur', function () {
        if ($(this).val() == '') {
            $(this).hide().siblings('.pwd-text').show();
        }
    });


    //注册处理
    $('.reg-box :text,.reg-box :password,.reg-box textarea').on({
        focus: function () {
            $(this).addClass('focus');
        },
        blur: function () {
            $(this).removeClass('focus');
        }
    });

    try {
        $(window).hashchange(function () {
            var aRegStep = $('.reg-step li');
            var oUserForm = $('.users-form');
            var oBaseForm = $('.base-form');
            var oShopForm = $('.shop-form');
            switch (location.hash) {
                case '#passuser':
                    aRegStep.eq(1).addClass('current').siblings().removeClass('current');
                    oUserForm.hide();
                    oBaseForm.show();
                    oShopForm.hide();
                    $("#select-city").citySelect({
                        prov: "北京",
                        nodata: "none"
                    });
                    break;
                case '#passbase':
                    aRegStep.eq(2).addClass('current').siblings().removeClass('current');
                    oUserForm.hide();
                    oBaseForm.hide();
                    oShopForm.show();
                    break;
                default:
                    aRegStep.eq(0).addClass('current').siblings().removeClass('current');
                    oUserForm.show();
                    oBaseForm.hide();
                    oShopForm.hide();
            }
        });
        $(window).hashchange();

    } catch (e) { }

    $('.pass-users').click(passusers);
    $('.users-form input').on('keyup', function (e) {
        if (e.which == 13) {
            passusers();
        }
    });

    $('.pass-base').click(passbase);
    $('.base-form input,.base-form textarea').on('keyup', function (e) {
        if (e.which == 13) {
            passbase();
        }
    });

    $('.shop-form .upload-file').change(function () {
        var txt = $(this).val();
        $(this).siblings('.upload-txt').html(txt);
    });

    $('.submit-all').click(function () {
        var aText = $('.shop-form .upload-txt');

        if ($('.consent-agreement :checked').length == 0) {
            alert('请同意微沙龙服务协议');
            return false;
        }
        CHKregister();
    });

    /*
        $('.reg-form :text').on('focus',function(){
            var _self = $(this);
            if(_self.val() === _self.prev().text()+'不能为空'){
                _self.removeClass('error').val('');
            }
        }).on('blur',function(){
            var _self = $(this);
            if(_self.val() === '' || _self.val() === _self.prev().text()+'不能为空'){
                _self.addClass('error').val(_self.prev().text()+'不能为空');
            }
        });
    */

    function passusers() {
        var aInput = $('.users-form input');
        var oName = aInput.eq(0);
        var oPass = aInput.eq(1);
        var oRePass = aInput.eq(2);
        var ipass = true;
        $.each(aInput, function () {
            var _self = $(this);
            var reg = /^\w+$/;
            if (_self.val() == '' || !reg.test(_self.val())) {
                _self.addClass('error').focus().nextAll('.tips').eq(0).addClass('error');
                ipass = false;
                return false;
            }
            if (_self.val().length < 6 && (_self.hasClass('password') || _self.hasClass('username'))) {
                _self.addClass('error').focus().nextAll('.tips').eq(0).addClass('error');
                ipass = false;
                return false;
            }
            _self.removeClass('error').nextAll('.tips').eq(0).removeClass('error');
        });
        if (!ipass) return false;

        if (oRePass.val() != oPass.val()) {
            oRePass.addClass('error').focus().nextAll('.tips').eq(0).addClass('error');
            return false;
        }

        $.ajax({
            type: 'post',
            dataType: 'text',
            url: '/Register/CHKLoginID/',
            data: { loginID: oName.val() }
        }).done(function (res) {
            if (res == 1) {
                location.hash = 'passuser';
            }
            else {
                oName.focus().addClass('error').nextAll('.tips').eq(0).html('用户名' + oName.val() + '已被使用').addClass('error');
            }
        });
    }

    function passbase() {
        var aInput = $('.base-form :text');
        var ipass = true;
        $.each(aInput, function () {
            var _self = $(this);
            if (_self.next('.required').length === 0) return true;
            if (_self.val() == '') {
                _self.addClass('error').focus().nextAll('.tips').eq(0).addClass('error');
                ipass = false;
                return false;
            }
            //电话手机检测
            if (_self.hasClass('telephone') && _self.val().search(/^0\d{2,3}-\d{7,8}$|^(13|15|17|18|19)\d{9}$/g) == -1) {
                _self.addClass('error').focus().nextAll('.tips').eq(0).addClass('error');
                ipass = false;
                return false;
            }
            _self.removeClass('error').nextAll('.tips').eq(0).removeClass('error');
        });
        if (!ipass) return false;

        location.hash = 'passbase';
    }

    //边栏登陆
    $('.tips-login a').click(function () {
        $('.tips-search').html('输入用户名和密码，立即登陆');
        $('.search-btn').val('登录');
        $('.search-form input[type=text]').eq(0).focus();
        return false;
    });

});

function Encrypt(word) {
    var test = new AES.Crypto('ihlih*0037JOHT*)(PIJY*(()JI^)IO%');
    return test.encrypt(word);
}

function Eecrypt(word) {
    var test = new AES.Crypto('ihlih*0037JOHT*)(PIJY*(()JI^)IO%');
    return test.decrypt(word);
}

var k = false;
//验证登陆
function checkLogin() {
    if (k) {
        return;
    }
    k = true;
    var loginID = Encrypt($('#logname').val());
    var PWD = Encrypt($('#pws').val());
    var param = {
        "loginID": loginID,
        "PWD": PWD
    };
    $.ajax({
        type: "post",
        url: "/Register/CHKLogin",
        dataType: "text",
        data: param,
        success: function (data) {
            if (data.length > 0) {
                var msg1 = data.split('|')[0];
                if (escape(msg1).indexOf("%u") < 0) {
                    if (msg1 == "2") {
                        alert("您的账号已过试用期，或者已过认证期。如继续使用，请支付认证费。");
                    }
                    window.location.href = data.split('|')[1];
                } else {
                    alert(msg1);
                    k = false;
                }
            } else {
                alert(data);
                k = false;
            }
        },
        error: function (data) {
            alert("失败-" + data.readyState);
            k = false;
        }
    });
}

function LoginOut() {
    $.ajax({
        type: "post",
        url: "/Register/LoginOut",
        dataType: "text",
        success: function (data) {
            window.location.href = "/Home/Index";
        },
        error: function (data) {
            alert("失败-" + data.readyState);
        }
    });
}