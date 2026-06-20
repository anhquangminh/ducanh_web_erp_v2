
function dinhDuongOngThuongLuu(dongChayThuongLuu, duongKinhNgoaiOngNhua, cDayOngOngNhua) {
    return Math.round((dongChayThuongLuu + duongKinhNgoaiOngNhua - cDayOngOngNhua) * 100) / 100;
}
function dinhDemCatThuongLuu(id_HinhThucDapTra, dongChayThuongLuu, cDayOngOngNhua) {
    const loaiHopLe = [
        "Đệm cát + đắp cát",
        "Đệm cát + đắp cát + đắp đất",
        "Đệm cát + đắp đất"
    ];

    if (loaiHopLe.includes(id_HinhThucDapTra)) {
        return Math.round((dongChayThuongLuu - cDayOngOngNhua) * 100) / 100;
    }
    return 0;
}
function cDoDayDemCatThuongLuu(id_HinhThucDapTra, dinhDemCatThuongLuu, cDayDemCatThuongLuu) {
    const loaiHopLe = [
        "Đệm cát + đắp cát",
        "Đệm cát + đắp cát + đắp đất",
        "Đệm cát + đắp đất"
    ];

    if (loaiHopLe.includes(id_HinhThucDapTra)) {
        return Math.round((dinhDemCatThuongLuu - cDayDemCatThuongLuu) * 100) / 100;
    }
    return 0;
}
function dayDaoThuongLuu(id_HinhThucDapTra, dongChayThuongLuu, cDayOngOngNhua, cDoDayDemCatThuongLuu) {
    const loai1 = ["Đắp đất", "Đắp đất + đắp cát", "Đắp cát + đắp đất", "Đắp cát"];
    const loai2 = ["Đệm cát + đắp cát", "Đệm cát + đắp cát + đắp đất", "Đệm cát + đắp đất"];

    if (loai1.includes(id_HinhThucDapTra)) {
        return Math.round((dongChayThuongLuu - cDayOngOngNhua) * 100) / 100;
    } else if (loai2.includes(id_HinhThucDapTra)) {
        return cDoDayDemCatThuongLuu;
    } else {
        return 0;
    }
}
function chieuSauDaoThuongLuu(hTrangTruocKhiDaoThuongLuu, dayDaoThuongLuu) {
    return hTrangTruocKhiDaoThuongLuu > 0 ? hTrangTruocKhiDaoThuongLuu - dayDaoThuongLuu : 0;
}
function tongChieuDayDemDapCatThuongLuu(cDayDemCatThuongLuu, cDayDapCatThuongLuu) {
    return Math.round((cDayDemCatThuongLuu + cDayDapCatThuongLuu) * 100) / 100;
}
function dapDatCatThuongLuu(tongChieuDayDemDapCatThuongLuu, chieuDayDapDatThuongLuu) {
    return Math.round((tongChieuDayDemDapCatThuongLuu + chieuDayDapDatThuongLuu) * 100) / 100;
}
function chenhDapSoVoiDaoThuongLuu(chieuSauDaoThuongLuu, dapDatCatThuongLuu) {
    return Math.round((chieuSauDaoThuongLuu - dapDatCatThuongLuu) * 100) / 100;
}
function dinhDuongOngHaLuu(dongChayHaLuu, duongKinhNgoaiOngNhua, cDayOngOngNhua) {
    return Math.round((dongChayHaLuu + duongKinhNgoaiOngNhua - cDayOngOngNhua) * 100) / 100;
}
function dinhDemCatHaLuu(id_HinhThucDapTra, dongChayHaLuu, cDayOngOngNhua) {
    const loai = ["Đệm cát + đắp cát", "Đệm cát + đắp cát + đắp đất", "Đệm cát + đắp đất"];
    return loai.includes(id_HinhThucDapTra) ? Math.round((dongChayHaLuu - cDayOngOngNhua) * 100) / 100 : 0;
}
function dinhDapCatThuongLuu(id_HinhThucDapTra, dayDapCatThuongLuu, cDayDapCatThuongLuu) {
    const loai = [
        "Đắp đất + đắp cát",
        "Đệm cát + đắp cát",
        "Đệm cát + đắp cát + đắp đất",
        "Đắp cát + đắp đất",
        "Đắp cát"
    ];
    return loai.includes(id_HinhThucDapTra) ? Math.round((dayDapCatThuongLuu + cDayDapCatThuongLuu) * 100) / 100 : 0;
}
function cDoDayDapDatThuongLuu(id_HinhThucDapTra, dongChayThuongLuu, cDayOngOngNhua, dinhDapCatThuongLuu, dinhDemCatThuongLuu) {
    const loai1 = ["Đắp đất", "Đắp đất + đắp cát"];
    const loai2 = ["Đệm cát + đắp cát + đắp đất", "Đắp cát + đắp đất"];

    if (loai1.includes(id_HinhThucDapTra)) {
        return Math.round((dongChayThuongLuu - cDayOngOngNhua) * 100) / 100;
    } else if (loai2.includes(id_HinhThucDapTra)) {
        return dinhDapCatThuongLuu;
    } else if (id_HinhThucDapTra === "Đệm cát + đắp đất") {
        return dinhDemCatThuongLuu;
    } else {
        return 0;
    }
}
function dinhDapDatThuongLuu(id_HinhThucDapTra, cDoDayDapDatThuongLuu, chieuDayDapDatThuongLuu) {
    const loai = [
        "Đắp đất",
        "Đắp đất + đắp cát",
        "Đệm cát + đắp cát + đắp đất",
        "Đệm cát + đắp đất",
        "Đắp cát + đắp đất"
    ];

    if (loai.includes(id_HinhThucDapTra)) {
        return Math.round((cDoDayDapDatThuongLuu + chieuDayDapDatThuongLuu) * 100) / 100;
    } else {
        return 0;
    }
}
function dayDapCatThuongLuu(id_HinhThucDapTra, dongChayThuongLuu, cDayOngOngNhua, dinhDemCatThuongLuu, dinhDapDatThuongLuu) {
    const loai1 = ["Đắp cát", "Đắp cát + đắp đất"];
    const loai2 = ["Đệm cát + đắp cát", "Đệm cát + đắp cát + đắp đất"];

    if (loai1.includes(id_HinhThucDapTra)) {
        return Math.round((dongChayThuongLuu - cDayOngOngNhua) * 100) / 100;
    } else if (loai2.includes(id_HinhThucDapTra)) {
        return dinhDemCatThuongLuu;
    } else if (id_HinhThucDapTra === "Đắp đất + đắp cát") {
        return dinhDapDatThuongLuu;
    } else {
        return 0;
    }
}
function dayDaoHaLuu(id_HinhThucDapTra, dongChayHaLuu, cDayOngOngNhua, cDoDayDemCatHaLuu) {
    const loai1 = ["Đắp đất", "Đắp đất + đắp cát", "Đắp cát + đắp đất", "Đắp cát"];
    const loai2 = ["Đệm cát + đắp cát", "Đệm cát + đắp cát + đắp đất", "Đệm cát + đắp đất"];

    if (loai1.includes(id_HinhThucDapTra)) {
        return Math.round((dongChayHaLuu - cDayOngOngNhua) * 100) / 100;
    } else if (loai2.includes(id_HinhThucDapTra)) {
        return cDoDayDemCatHaLuu;
    } else {
        return 0;
    }
}
function chieuSauDaoHaLuu(hTrangTruocKhiDaoHaLuu, dayDaoHaLuu) {
    return hTrangTruocKhiDaoHaLuu > 0 ? Math.round((hTrangTruocKhiDaoHaLuu - dayDaoHaLuu) * 100) / 100 : 0;
}
function cDoDayDemCatHaLuu(id_HinhThucDapTra, dinhDemCatHaLuu, cDayDemCatHaLuu) {
    const loai = ["Đệm cát + đắp cát", "Đệm cát + đắp cát + đắp đất", "Đệm cát + đắp đất"];
    return loai.includes(id_HinhThucDapTra) ? Math.round((dinhDemCatHaLuu - cDayDemCatHaLuu) * 100) / 100 : 0;
}
function tongChieuDayDemDapCatHaLuu(cDayDemCatHaLuu, cDayDapCatHaLuu) {
    return Math.round((cDayDemCatHaLuu + cDayDapCatHaLuu) * 100) / 100;
}
function dinhDapDatHaLuu(id_HinhThucDapTra, cDoDayDapDatHaLuu, chieuDayDapDatHaLuu) {
    const loai = [
        "Đắp đất",
        "Đắp đất + đắp cát",
        "Đệm cát + đắp cát + đắp đất",
        "Đệm cát + đắp đất",
        "Đắp cát + đắp đất"
    ];

    if (loai.includes(id_HinhThucDapTra)) {
        return Math.round((cDoDayDapDatHaLuu + chieuDayDapDatHaLuu) * 100) / 100;
    } else {
        return 0;
    }
}
function dayDapCatHaLuu(id_HinhThucDapTra, dongChayHaLuu, cDayOngOngNhua, dinhDemCatHaLuu, dinhDapDatHaLuu) {
    const loai1 = ["Đắp cát", "Đắp cát + đắp đất"];
    const loai2 = ["Đệm cát + đắp cát", "Đệm cát + đắp cát + đắp đất"];

    if (loai1.includes(id_HinhThucDapTra)) {
        return Math.round((dongChayHaLuu - cDayOngOngNhua) * 100) / 100;
    } else if (loai2.includes(id_HinhThucDapTra)) {
        return dinhDemCatHaLuu;
    } else if (id_HinhThucDapTra === "Đắp đất + đắp cát") {
        return dinhDapDatHaLuu;
    } else {
        return 0;
    }
}
function dinhDapCatHaLuu(id_HinhThucDapTra, dayDapCatHaLuu, cDayDapCatHaLuu) {
    const loai = [
        "Đắp đất + đắp cát",
        "Đệm cát + đắp cát",
        "Đệm cát + đắp cát + đắp đất",
        "Đắp cát + đắp đất",
        "Đắp cát"
    ];

    if (loai.includes(id_HinhThucDapTra)) {
        return Math.round((dayDapCatHaLuu + cDayDapCatHaLuu) * 100) / 100;
    } else {
        return 0;
    }
}
function cDoDayDapDatHaLuu(id_HinhThucDapTra, dongChayHaLuu, cDayOngOngNhua, dinhDapCatHaLuu, dinhDemCatHaLuu) {
    const loai1 = ["Đắp đất", "Đắp đất + đắp cát"];
    const loai2 = ["Đệm cát + đắp cát + đắp đất", "Đắp cát + đắp đất"];

    if (loai1.includes(id_HinhThucDapTra)) {
        return Math.round((dongChayHaLuu - cDayOngOngNhua) * 100) / 100;
    } else if (loai2.includes(id_HinhThucDapTra)) {
        return dinhDapCatHaLuu;
    } else if (id_HinhThucDapTra === "Đệm cát + đắp đất") {
        return dinhDemCatHaLuu;
    } else {
        return 0;
    }
}
function dapDatCatHaLuu(tongChieuDayDemDapCatHaLuu, chieuDayDapDatHaLuu) {
    return Math.round((tongChieuDayDemDapCatHaLuu + chieuDayDapDatHaLuu) * 100) / 100;
}
function chenhDapSoVoiDaoHaLuu(chieuSauDaoHaLuu, dapDatCatHaLuu) {
    return Math.round((chieuSauDaoHaLuu - dapDatCatHaLuu) * 100) / 100;
}
function cDoDayDemCat(cDoDayDemCatThuongLuu, cDoDayDemCatHaLuu) {
    return Math.round(((cDoDayDemCatThuongLuu + cDoDayDemCatHaLuu) / 2) * 100) / 100;
}
function cDoDinhDemCat(dinhDemCatThuongLuu, dinhDemCatHaLuu) {
    return Math.round(((dinhDemCatThuongLuu + dinhDemCatHaLuu) / 2) * 100) / 100;
}
function chieuDayDemCat(cDoDinhDemCat, cDoDayDemCat) {
    return Math.round((cDoDinhDemCat - cDoDayDemCat) * 100) / 100;
}
function cDoDayDapCat(dayDapCatThuongLuu, dayDapCatHaLuu) {
    return Math.round(((dayDapCatThuongLuu + dayDapCatHaLuu) / 2) * 100) / 100;
}
function cDoDinhDapCat(dinhDapCatThuongLuu, dinhDapCatHaLuu) {
    return Math.round(((dinhDapCatThuongLuu + dinhDapCatHaLuu) / 2) * 100) / 100;
}
function chieuDayDapCat(cDoDinhDapCat, cDoDayDapCat) {
    return Math.round((cDoDinhDapCat - cDoDayDapCat) * 100) / 100;
}
function cDoDayDapDat(cDoDayDapDatThuongLuu, cDoDayDapDatHaLuu) {
    return Math.round(((cDoDayDapDatThuongLuu + cDoDayDapDatHaLuu) / 2) * 100) / 100;
}
function cDoDinhDapDat(dinhDapDatThuongLuu, dinhDapDatHaLuu) {
    return Math.round(((dinhDapDatThuongLuu + dinhDapDatHaLuu) / 2) * 100) / 100;
}
function chieuDayDapDat(cDoDinhDapDat, cDoDayDapDat) {
    return Math.round((cDoDinhDapDat - cDoDayDapDat) * 100) / 100;
}
function cRongDayNhoTBinh(cRongDayNhoHLuu, cRongDayNhoTLuu) {
    return Math.round(((cRongDayNhoHLuu + cRongDayNhoTLuu) / 2) * 100) / 100;
}
function chieuSauDaoTrungBinh(chieuSauDaoThuongLuu, chieuSauDaoHaLuu) {
    return Math.round(((chieuSauDaoThuongLuu + chieuSauDaoHaLuu) / 2) * 100) / 100;
}
function cRongDayLonTrungBinh(chieuSauDaoTrungBinh, tyLeMoMai, soMaiTrai, soMaiPhai, cRongDayNhoTBinh) {
    return Math.round((chieuSauDaoTrungBinh * tyLeMoMai * (soMaiTrai + soMaiPhai) + cRongDayNhoTBinh) * 100) / 100;
}
function dienTich(cRongDayNhoTBinh, cRongDayLonTrungBinh, chieuSauDaoTrungBinh) {
    if (cRongDayNhoTBinh <= 0) return 0;
    return Math.round(((cRongDayNhoTBinh + cRongDayLonTrungBinh) * chieuSauDaoTrungBinh) / 2 * 100) / 100;
}
function klDao(dienTich, chieuDaiOngNhua) {
    return Math.round(dienTich * chieuDaiOngNhua * 100) / 100;
}
function cRongDayNhoDemCat(id_HinhThucDapTra, cRongDayNhoTBinh) {
    const loai = ["Đệm cát + đắp cát", "Đệm cát + đắp cát + đắp đất", "Đệm cát + đắp đất"];
    return loai.includes(id_HinhThucDapTra) ? cRongDayNhoTBinh : 0;
}
function cRongDayLonDemCat(chieuDayDemCat, tyLeMoMai, soMaiTrai, soMaiPhai, cRongDayNhoDemCat) {
    return Math.round((chieuDayDemCat * tyLeMoMai * (soMaiTrai + soMaiPhai) + cRongDayNhoDemCat) * 100) / 100;
}
function dienTichDapCat1(cRongDayNhoDemCat, cRongDayLonDemCat, chieuDayDemCat) {
    return Math.round(((cRongDayNhoDemCat + cRongDayLonDemCat) * chieuDayDemCat) / 2 * 100) / 100;
}
function klDemCat(dienTichDapCat1, chieuDaiOngNhua) {
    return Math.round(dienTichDapCat1 * chieuDaiOngNhua * 100) / 100;
}
function cRongDayNhoDapCat(id_HinhThucDapTra, cDoDinhDemCat, cDoDayDemCat, tyLeMoMai, soMaiTrai, soMaiPhai, cRongDayNhoTBinh, chieuDayDapDat) {
    const loai1 = ["Đệm cát + đắp cát", "Đệm cát + đắp cát + đắp đất"];
    const loai2 = ["Đắp cát + đắp đất", "Đắp cát"];

    if (loai1.includes(id_HinhThucDapTra)) {
        return Math.round(((cDoDinhDemCat - cDoDayDemCat) * tyLeMoMai * (soMaiTrai + soMaiPhai)) + cRongDayNhoTBinh * 100) / 100;
    } else if (loai2.includes(id_HinhThucDapTra)) {
        return cRongDayNhoTBinh;
    } else if (id_HinhThucDapTra === "Đắp đất + đắp cát") {
        return Math.round((chieuDayDapDat * tyLeMoMai * (soMaiTrai + soMaiPhai)) + cRongDayNhoTBinh * 100) / 100;
    } else {
        return 0;
    }
}
function cRongDayLonDapCat(chieuDayDapCat, tyLeMoMai, soMaiTrai, soMaiPhai, cRongDayNhoDapCat) {
    return Math.round((chieuDayDapCat * tyLeMoMai * (soMaiTrai + soMaiPhai) + cRongDayNhoDapCat) * 100) / 100;
}
function dienTichDapCat2(cRongDayNhoDapCat, cRongDayLonDapCat, chieuDayDapCat) {
    return Math.round(((cRongDayNhoDapCat + cRongDayLonDapCat) * chieuDayDapCat) / 2 * 100) / 100;
}
function klDapCat(dienTichDapCat2, chieuDaiOngNhua) {
    return Math.round(dienTichDapCat2 * chieuDaiOngNhua * 100) / 100;
}
function kLDapCat_KlOngCCho(cDoDayDapCat, cDoDinhDapCat, dongChayThuongLuu, dongChayHaLuu, cDayOngOngNhua, dinhDuongOngThuongLuu, dinhDuongOngHaLuu, duongKinhNgoaiOngNhua, chieuDaiOngNhua) {
    const avg1 = (dongChayThuongLuu + dongChayHaLuu) / 2 - cDayOngOngNhua;
    const avg2 = (dinhDuongOngThuongLuu + dinhDuongOngHaLuu) / 2;

    let numerator = 0;

    if (cDoDayDapCat >= avg1 && cDoDinhDapCat <= avg2) {
        numerator = cDoDinhDapCat - cDoDayDapCat;
    } else if (cDoDayDapCat < avg2 && cDoDinhDapCat > avg2) {
        numerator = avg2 - cDoDayDapCat;
    } else {
        numerator = 0;
    }

    const fraction = numerator / duongKinhNgoaiOngNhua;
    const area = (3.14 * duongKinhNgoaiOngNhua * duongKinhNgoaiOngNhua) / 4;

    return Math.round(fraction * area * chieuDaiOngNhua * 100) / 100;
}
function klDapCatSauCCho(klDapCat, kLDapCat_KlOngCCho) {
    return Math.round((klDapCat - kLDapCat_KlOngCCho) * 100) / 100;
}
function cRongDayNhoDapDat(id_HinhThucDapTra, cRongDayNhoTBinh, cDoDinhDemCat, cDoDayDemCat, tyLeMoMai, soMaiTrai, soMaiPhai, cDoDinhDapCat, cDoDayDapCat, chieuDayDemCat, chieuDayDapCat) {
    if (id_HinhThucDapTra === "Đắp đất" || id_HinhThucDapTra === "Đắp đất + đắp cát") {
        return cRongDayNhoTBinh;
    }
    if (id_HinhThucDapTra === "Đệm cát + đắp đất") {
        return Math.round(((cDoDinhDemCat - cDoDayDemCat) * tyLeMoMai * (soMaiTrai + soMaiPhai) + cRongDayNhoTBinh) * 100) / 100;
    }
    if (id_HinhThucDapTra === "Đắp cát + đắp đất") {
        return Math.round(((cDoDinhDapCat - cDoDayDapCat) * tyLeMoMai * (soMaiTrai + soMaiPhai) + cRongDayNhoTBinh) * 100) / 100;
    }
    if (id_HinhThucDapTra === "Đệm cát + đắp cát + đắp đất") {
        return Math.round(((chieuDayDemCat + chieuDayDapCat) * tyLeMoMai * (soMaiTrai + soMaiPhai) + cRongDayNhoTBinh) * 100) / 100;
    }
    return 0;
}
function cRongDayLonDapDat(chieuDayDapDat, tyLeMoMai, soMaiTrai, soMaiPhai, cRongDayNhoDapDat) {
    return Math.round((chieuDayDapDat * tyLeMoMai * (soMaiTrai + soMaiPhai) + cRongDayNhoDapDat) * 100) / 100;
}
function dienTichDapDat(cRongDayNhoDapDat, cRongDayLonDapDat, chieuDayDapDat) {
    return Math.round(((cRongDayNhoDapDat + cRongDayLonDapDat) * chieuDayDapDat / 2) * 100) / 100;
}
function klDapDat(dienTichDapDat, chieuDaiOngNhua) {
    return Math.round(dienTichDapDat * chieuDaiOngNhua * 100) / 100;
}
function kLDapDat_KlOngCCho(cDoDayDapDat, cDoDinhDapDat, dongChayThuongLuu, dongChayHaLuu, cDayOngOngNhua, dinhDuongOngThuongLuu, dinhDuongOngHaLuu, duongKinhNgoaiOngNhua, chieuDaiOngNhua) {
    const halfX = (dongChayThuongLuu + dongChayHaLuu) / 2 - cDayOngOngNhua;
    const halfY = (dinhDuongOngThuongLuu + dinhDuongOngHaLuu) / 2;
    let numerator = 0;

    if (cDoDayDapDat >= halfX && cDoDinhDapDat <= halfY) {
        numerator = cDoDinhDapDat - cDoDayDapDat;
    } else if (cDoDayDapDat < halfY && cDoDinhDapDat > halfY) {
        numerator = halfY - cDoDayDapDat;
    } else {
        numerator = 0;
    }

    const ratio = numerator / duongKinhNgoaiOngNhua;
    const area = (3.14 * duongKinhNgoaiOngNhua * duongKinhNgoaiOngNhua) / 4;

    return Math.round(ratio * area * chieuDaiOngNhua * 100) / 100;
}
function klDapDatSauCCho(klDapDat, kLDapDat_KlOngCCho) {
    return Math.round((klDapDat - kLDapDat_KlOngCCho) * 100) / 100;
}
function klDatThua(kLDapCat_KlOngCCho, kLDapDat_KlOngCCho) {
    return Math.round((kLDapCat_KlOngCCho + kLDapDat_KlOngCCho) * 100) / 100;
}
