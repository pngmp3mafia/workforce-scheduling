import { useGetEmployeesQuery, useAddEmployeeMutation, useGetShiftsQuery, useAddShiftMutation } from './services/api'
import { useState } from 'react'

export default function App() {
  const { data: employees } = useGetEmployeesQuery()
  const [addEmployee] = useAddEmployeeMutation()

  const { data: shifts } = useGetShiftsQuery()
  const [addShift] = useAddShiftMutation()

  // employee form
  const [first, setFirst] = useState('Jordan')
  const [last, setLast] = useState('Lee')
  const [skills, setSkills] = useState('Cashier,Inventory')

  // shift form
  const [date, setDate] = useState('2025-08-14')
  const [start, setStart] = useState('09:00')
  const [end, setEnd] = useState('17:00')
  const [req, setReq] = useState('Cashier:1;Forklift:1')

  return (
    <div style={{fontFamily:'system-ui', padding:24, maxWidth:900, margin:'0 auto'}}>
      <h1>Workforce Scheduling — Dev UI</h1>

      <section style={{marginBottom:24}}>
        <h2>Employees</h2>
        <div style={{display:'flex', gap:8, marginBottom:12}}>
          <input value={first} onChange={e=>setFirst(e.target.value)} placeholder="First"/>
          <input value={last} onChange={e=>setLast(e.target.value)} placeholder="Last"/>
          <input value={skills} onChange={e=>setSkills(e.target.value)} placeholder="Skills (comma sep)"/>
          <button onClick={()=>{
            const body = { firstName:first, lastName:last, skills: skills.split(',').map(s=>s.trim()).filter(Boolean) }
            addEmployee(body)
          }}>Add Employee</button>
        </div>
        <ul>
          {employees?.map(e=>(
            <li key={e.id}>{e.name} — [{e.skills.join(', ')}]</li>
          ))}
        </ul>
      </section>

      <section>
        <h2>Shifts</h2>
        <div style={{display:'flex', gap:8, marginBottom:12}}>
          <input value={date} onChange={e=>setDate(e.target.value)} placeholder="YYYY-MM-DD"/>
          <input value={start} onChange={e=>setStart(e.target.value)} placeholder="HH:mm"/>
          <input value={end} onChange={e=>setEnd(e.target.value)} placeholder="HH:mm"/>
          <input value={req} onChange={e=>setReq(e.target.value)} placeholder="name:count;name:count"/>
          <button onClick={()=>{
            const requiredSkills = req.split(';').map(p=>{
              const [name,count] = p.split(':')
              return { name: (name||'').trim(), count: Math.max(1, parseInt(count||'1',10)) }
            }).filter(r=>r.name)
            addShift({ date, start, end, requiredSkills })
          }}>Add Shift</button>
        </div>
        <ul>
          {shifts?.map(s=>(
            <li key={s.id}>{s.date} {s.start}–{s.end} — req: [{s.required.map(r=>`${r.name}:${r.count}`).join(', ')}]</li>
          ))}
        </ul>
      </section>
    </div>
  )
}
