import { PieChart, Pie, Cell, Tooltip, ResponsiveContainer } from 'recharts'

// Pure presentational component: takes data, renders a chart.
// It never fetches data itself - the page that renders it owns that.
//
// data shape: [{ categoryName, categoryColor, total }, ...]
// (ASP.NET Core's default JSON serializer camelCases property names,
// so "CategoryName" on the C# record arrives as "categoryName" here.)
export default function SummaryChart({ data }) {
  if (!data || data.length === 0) {
    return <p className="muted">No expenses to chart yet.</p>
  }

  return (
    <ResponsiveContainer width="100%" height={220}>
      <PieChart>
        <Pie
          data={data}
          dataKey="total"
          nameKey="categoryName"
          innerRadius={50}
          outerRadius={90}
          paddingAngle={2}
        >
          {data.map((entry) => (
            <Cell key={entry.categoryName} fill={entry.categoryColor} />
          ))}
        </Pie>
        <Tooltip formatter={(value) => `\u20a6${value.toFixed(2)}`} />
      </PieChart>
    </ResponsiveContainer>
  )
}
