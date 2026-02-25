const fs=require("fs");
const BT=String.fromCharCode(96);
const BASE="D:/project/assetmanage/Asset-Management-System/ClientApp/app";
function w(fp,lines){fs.writeFileSync(fp,lines.join("
")+"
","utf8");console.log("Written:",fp);}
const borrowLines=[];
const bl=(s)=>borrowLines.push(s);
bl('"use client";')
bl('import { useState, useEffect, useCallback } from "react";')
bl('import MainLayout from "../components/MainLayout";')
bl('import Header from "../components/Header";')
bl('import { borrowApi, assetApi, BorrowRequest, AssetSummary } from "../lib/api";')
bl('')
bl('const STATUS_COLORS: Record<string, string> = {')
bl('  Pending: "bg-yellow-100 text-yellow-700",')
bl('  Approved: "bg-blue-100 text-blue-700",')
bl('  Returned: "bg-green-100 text-green-700",')
bl('  Rejected: "bg-red-100 text-red-700",')
bl('};')
bl('')
bl('export default function BorrowPage() {')
