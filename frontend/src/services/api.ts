import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

const baseUrl = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5005';

export interface Employee { id: string; name: string; skills: string[]; }
export interface CreateEmployee { firstName: string; lastName: string; skills: string[]; }

export interface Shift {
  id: string; date: string; start: string; end: string;
  required: { name: string; count: number }[];
}
export interface CreateShift {
  date: string; start: string; end: string;
  requiredSkills: { name: string; count: number }[];
}

export const api = createApi({
  reducerPath: 'api',
  baseQuery: fetchBaseQuery({ baseUrl }),
  tagTypes: ['Employees', 'Shifts'],
  endpoints: (builder) => ({
    getEmployees: builder.query<Employee[], void>({
      query: () => '/employees',
      providesTags: ['Employees'],
    }),
    addEmployee: builder.mutation<Employee, CreateEmployee>({
      query: (body) => ({ url: '/employees', method: 'POST', body }),
      invalidatesTags: ['Employees'],
    }),
    getShifts: builder.query<Shift[], void>({
      query: () => '/shifts',
      providesTags: ['Shifts'],
    }),
    addShift: builder.mutation<Shift, CreateShift>({
      query: (body) => ({ url: '/shifts', method: 'POST', body }),
      invalidatesTags: ['Shifts'],
    }),
  }),
});

export const {
  useGetEmployeesQuery,
  useAddEmployeeMutation,
  useGetShiftsQuery,
  useAddShiftMutation,
} = api;
