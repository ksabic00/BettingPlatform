import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { isPlatformBrowser } from '@angular/common';
import { OfferDto, OfferOutcomeDto } from '../../api';

export interface SlipSelection {
  offerId: string;
  matchId: string;                 
  outcomeTemplateId: string;
  marketCode: string;
  outcomeCode: string;
  odds: number;
  match: string;               
  startsAtUtc?: string;
}

type Persisted = { stake: number; selections: SlipSelection[] };

const KEY = 'betSlip';

@Injectable({ providedIn: 'root' })
export class BetslipService {
  private readonly _selections = new BehaviorSubject<SlipSelection[]>([]);
  private readonly _stake = new BehaviorSubject<number>(10);

  readonly selections$ = this._selections.asObservable();
  readonly stake$ = this._stake.asObservable();

  constructor(@Inject(PLATFORM_ID) private pid: Object) {
    if (isPlatformBrowser(pid)) {
      try {
        const raw = localStorage.getItem(KEY);
        if (raw) {
          const data = JSON.parse(raw) as Persisted;
          if (typeof data?.stake === 'number') this._stake.next(data.stake);
          if (Array.isArray(data?.selections)) this._selections.next(data.selections);
        }
      } catch { }

      this._selections.subscribe(() => this.persist());
      this._stake.subscribe(() => this.persist());
      this.persist();
    }
  }

  private persist() {
    if (!isPlatformBrowser(this.pid)) return;
    const data: Persisted = {
      stake: this._stake.value,
      selections: this._selections.value
    };
    try { localStorage.setItem(KEY, JSON.stringify(data)); } catch {  }
  }

  clear() { this._selections.next([]); }

  setStake(v: number) {
    const n = Number.isFinite(v) ? Number(v) : 0;
    this._stake.next(Math.max(0, n));
  }


  addOrReplace(offer: OfferDto, outcome: OfferOutcomeDto) {
    const s: SlipSelection = {
      offerId: offer.offerId!,
      matchId: offer.matchId!,                 
      outcomeTemplateId: outcome.outcomeTemplateId!,
      marketCode: offer.marketCode!,
      outcomeCode: outcome.outcomeCode!,
      odds: Number(outcome.odds),
      match: `${offer.homeTeam} vs ${offer.awayTeam}`,
      startsAtUtc: offer.startsAtUtc as any
    };

    const list = this._selections.value;

    const withoutSameMatch = list.filter(x => x.matchId !== s.matchId);

    this._selections.next([...withoutSameMatch, s]);
  }


  toggle(offer: OfferDto, outcome: OfferOutcomeDto) {
    const current = this._selections.value;
    const matchId = offer.matchId!;
    const same = current.find(x =>
      x.matchId === matchId && x.outcomeTemplateId === outcome.outcomeTemplateId
    );

    if (same) {
      this._selections.next(current.filter(x => x !== same));
    } else {
      this.addOrReplace(offer, outcome);
    }
  }

  removeByOffer(offerId: string) {
    this._selections.next(this._selections.value.filter(x => x.offerId !== offerId));
  }

  removeByMatch(matchId: string) {
    this._selections.next(this._selections.value.filter(x => x.matchId !== matchId));
  }

  combinedOdds(): number {
    const prod = this._selections.value.reduce((acc, s) => acc * Number(s.odds || 1), 1);
    return Math.round(prod * 100) / 100;
  }

  getSnapshot() {
    return { stake: this._stake.value, selections: this._selections.value };
  }
}
