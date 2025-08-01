$(document).ready(function () {

    $('.numberonly').keypress(function (e) {
        let charCode = (e.which) ? e.which : event.keyCode
        if (String.fromCharCode(charCode).match(/[^0-9.]/g))
            return false;
    });
}); 
function makeid(length) {
    let text = "";
    const crypto = window.crypto || window.msCrypto;
    let array = new Uint32Array(1);
    text += crypto.getRandomValues(array);

    return text;
}
function LoginEnc(SheKey) {
    let txtUserName = $("#UserName").val();
    let txtPasswd = $("#Password").val();
    
    let encKey = makeid(16);

    let key = CryptoJS.enc.Utf8.parse(encKey);
    let iv = CryptoJS.enc.Utf8.parse(encKey);

    //Rev 30.0
    let encrypteduserId = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(txtUserName), key,
        {
            keySize: 128 / 8,
            iv: iv,
            mode: CryptoJS.mode.CBC,
            padding: CryptoJS.pad.Pkcs7
        });
    $("#UserName").val(encrypteduserId);
    $("#UserName").attr('type', 'password');
    let encryptedpasswd = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(txtPasswd), key,
        {
            keySize: 128 / 8,
            iv: iv,
            mode: CryptoJS.mode.CBC,
            padding: CryptoJS.pad.Pkcs7
        });
    $("#Password").val(encryptedpasswd);

    let key1 = CryptoJS.enc.Utf8.parse(SheKey);
    let iv1 = CryptoJS.enc.Utf8.parse(SheKey);
    let encryptedSheuserId = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(txtUserName), key1,
        {
            keySize: 128 / 8,
            iv: iv1,
            mode: CryptoJS.mode.CBC,
            padding: CryptoJS.pad.Pkcs7
        });
    $("#NameAseUsr").val(encryptedSheuserId);
    let encryptedShepasswd = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(txtPasswd), key1,
        {
            keySize: 128 / 8,
            iv: iv1,
            mode: CryptoJS.mode.CBC,
            padding: CryptoJS.pad.Pkcs7
        });
    $("#PasAseUsr").val(encryptedShepasswd);
}

function Encrypdata(SheKey, stringVal) {

    let key = CryptoJS.enc.Utf8.parse(SheKey);
    let iv = CryptoJS.enc.Utf8.parse(SheKey);    
    let encrypteduserId = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(stringVal), key,
        {
            keySize: 128 / 8,
            iv: iv,
            mode: CryptoJS.mode.CBC,
            padding: CryptoJS.pad.Pkcs7
        });    
    return encrypteduserId.toString();
}
function Decryptdata(SheKey, encryptedVal) {
    let key = CryptoJS.enc.Utf8.parse(SheKey);
    let iv = CryptoJS.enc.Utf8.parse(SheKey);

    let decrypted = CryptoJS.AES.decrypt(encryptedVal, key, {
        keySize: 128 / 8,
        iv: iv,
        mode: CryptoJS.mode.CBC,
        padding: CryptoJS.pad.Pkcs7
    });

    return decrypted.toString(CryptoJS.enc.Utf8);
}
$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip()
})


