import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export type CurrentPlayer = { id: string; displayName: string } | null;
const LS_KEY = 'bp.currentPlayer';

@Injectable({ providedIn: 'root' })
export class PlayerContextService {
  private readonly subj = new BehaviorSubject<CurrentPlayer>(this.read());
  current$ = this.subj.asObservable();

  set(p: CurrentPlayer) {
    this.subj.next(p);
    if (p) localStorage.setItem(LS_KEY, JSON.stringify(p));
    else localStorage.removeItem(LS_KEY);
  }

  get snapshot(): CurrentPlayer { return this.subj.value; }

  private read(): CurrentPlayer {
    try { return JSON.parse(localStorage.getItem(LS_KEY) || 'null'); }
    catch { return null; }
  }
}
