import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class StorageService {

  constructor() { }

  setItem(key:string, value:any){
    localStorage.setItem(key, JSON.stringify(value))
  }

  getItem(key:any) {
    const stored = localStorage.getItem(key)
    if(stored){
      return JSON.parse(stored)
    }
    return null
  }

  remove(key:any) {
    localStorage.removeItem(key);
  }

}
