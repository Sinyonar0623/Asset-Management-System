import MainLayout from "../components/MainLayout";
import Header from "../components/Header";

const monthlyStats = [
  { month: "Aug 2023", borrow: 18, repair: 4, returned: 15 },
  { month: "Sep 2023", borrow: 22, repair: 6, returned: 20 },
  { month: "Oct 2023", borrow: 31, repair: 3, returned: 28 },
  { month: "Nov 2023", borrow: 27, repair: 7, returned: 25 },
  { month: "Dec 2023", borrow: 14, repair: 2, returned: 14 },
  { month: "Jan 2024", borrow: 35, repair: 8, returned: 30 },
];

const topAssets = [
  { name: "Dell Laptop Latitude 5420", category: "Computers", borrowCount: 24, repairCount: 1 },
  { name: "Arduino Uno Kit", category: "Lab Equipment", borrowCount: 18, repairCount: 0 },
  { name: "Raspberry Pi 4 Kit", category: "Lab Equipment", borrowCount: 15, repairCount: 2 },
  { name: "Epson Projector EB-X51", category: "Audio/Visual", borrowCount: 12, repairCount: 3 },
  { name: "Oscilloscope Rigol DS1054Z", category: "Electronic", borrowCount: 10, repairCount: 4 },
];

const maxBorrow = Math.max(...monthlyStats.map(m => m.borrow));

export default function ReportsPage() {
  return (
    <MainLayout>
      <Header
        title="Reports"
        subtitle="สรุปรายงานการใช้งานทรัพย์สิน"
        actions={
          <button className="flex items-center gap-2 px-4 py-2 border border-slate-200 text-slate-700 rounded-lg text-sm font-medium hover:bg-slate-50">
            📥 Export PDF
          </button>
        }
      />

      <div className="flex-1 p-6 space-y-6">
        {/* Summary Cards */}
        <div className="grid grid-cols-2 sm:grid-cols-4 gap-4">
          {[
            { label: "Total Borrow (Jan)", value: "35", sub: "+26% from Dec", color: "blue" },
            { label: "Total Repair (Jan)", value: "8", sub: "+300% from Dec", color: "orange" },
            { label: "On-time Returns", value: "86%", sub: "30 / 35 requests", color: "green" },
            { label: "Overdue Returns", value: "5", sub: "Need follow-up", color: "red" },
          ].map((s) => (
            <div key={s.label} className="bg-white rounded-xl border border-slate-200 p-5 shadow-sm">
              <div className={`text-2xl font-bold ${
                s.color === "blue" ? "text-blue-600" :
                s.color === "orange" ? "text-orange-600" :
                s.color === "green" ? "text-green-600" : "text-red-600"
              }`}>{s.value}</div>
              <div className="text-sm font-medium text-slate-700 mt-1">{s.label}</div>
              <div className="text-xs text-slate-400 mt-0.5">{s.sub}</div>
            </div>
          ))}
        </div>

        <div className="grid grid-cols-1 xl:grid-cols-2 gap-6">
          {/* Monthly Chart */}
          <div className="bg-white rounded-xl border border-slate-200 shadow-sm">
            <div className="px-5 py-4 border-b border-slate-100">
              <h2 className="font-semibold text-slate-800">Monthly Activity</h2>
              <p className="text-xs text-slate-400 mt-0.5">Aug 2023 – Jan 2024</p>
            </div>
            <div className="p-5">
              <div className="flex items-end gap-2 h-40">
                {monthlyStats.map((m) => (
                  <div key={m.month} className="flex-1 flex flex-col items-center gap-1">
                    <div className="w-full flex flex-col items-center gap-0.5">
                      <div
                        className="w-full bg-blue-400 rounded-t"
                        style={{ height: `${(m.borrow / maxBorrow) * 100}px` }}
                        title={`Borrow: ${m.borrow}`}
                      />
                      <div
                        className="w-full bg-orange-300 rounded-t"
                        style={{ height: `${(m.repair / maxBorrow) * 100}px` }}
                        title={`Repair: ${m.repair}`}
                      />
                    </div>
                    <div className="text-xs text-slate-400 text-center leading-tight">{m.month.split(" ")[0]}</div>
                  </div>
                ))}
              </div>
              <div className="flex gap-4 mt-3 text-xs text-slate-500">
                <div className="flex items-center gap-1.5"><span className="w-3 h-3 bg-blue-400 rounded inline-block" /> Borrow</div>
                <div className="flex items-center gap-1.5"><span className="w-3 h-3 bg-orange-300 rounded inline-block" /> Repair</div>
              </div>
            </div>
          </div>

          {/* Top Used Assets */}
          <div className="bg-white rounded-xl border border-slate-200 shadow-sm">
            <div className="px-5 py-4 border-b border-slate-100">
              <h2 className="font-semibold text-slate-800">Most Used Assets</h2>
              <p className="text-xs text-slate-400 mt-0.5">All time ranking</p>
            </div>
            <div className="p-5 space-y-3">
              {topAssets.map((a, i) => (
                <div key={a.name} className="flex items-center gap-3">
                  <div className={`w-6 h-6 rounded-full flex items-center justify-center text-xs font-bold ${
                    i === 0 ? "bg-yellow-100 text-yellow-700" :
                    i === 1 ? "bg-slate-200 text-slate-600" :
                    i === 2 ? "bg-orange-100 text-orange-600" : "bg-slate-100 text-slate-500"
                  }`}>
                    {i + 1}
                  </div>
                  <div className="flex-1 min-w-0">
                    <div className="text-sm font-medium text-slate-800 truncate">{a.name}</div>
                    <div className="text-xs text-slate-400">{a.category}</div>
                  </div>
                  <div className="text-right">
                    <div className="text-sm font-semibold text-blue-600">{a.borrowCount}x</div>
                    <div className="text-xs text-orange-500">{a.repairCount} repairs</div>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>

        {/* Monthly Table */}
        <div className="bg-white rounded-xl border border-slate-200 shadow-sm">
          <div className="px-5 py-4 border-b border-slate-100">
            <h2 className="font-semibold text-slate-800">Monthly Summary Table</h2>
          </div>
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="bg-slate-50 border-b border-slate-100 text-left">
                  <th className="px-5 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Month</th>
                  <th className="px-5 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Borrow Requests</th>
                  <th className="px-5 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Repair Requests</th>
                  <th className="px-5 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Returned</th>
                  <th className="px-5 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wide">Return Rate</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-50">
                {monthlyStats.map((m) => (
                  <tr key={m.month} className="hover:bg-slate-50">
                    <td className="px-5 py-3 font-medium text-slate-800">{m.month}</td>
                    <td className="px-5 py-3 text-blue-600 font-semibold">{m.borrow}</td>
                    <td className="px-5 py-3 text-orange-600 font-semibold">{m.repair}</td>
                    <td className="px-5 py-3 text-green-600 font-semibold">{m.returned}</td>
                    <td className="px-5 py-3">
                      <div className="flex items-center gap-2">
                        <div className="w-20 bg-slate-100 rounded-full h-1.5">
                          <div className="bg-green-400 h-1.5 rounded-full" style={{ width: `${(m.returned / m.borrow) * 100}%` }} />
                        </div>
                        <span className="text-xs text-slate-600">{Math.round((m.returned / m.borrow) * 100)}%</span>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </MainLayout>
  );
}
